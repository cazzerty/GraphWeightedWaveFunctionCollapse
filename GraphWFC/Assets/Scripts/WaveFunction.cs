using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using UnityEngine;
using Tile = UnityEngine.Tilemaps.Tile;

public class WaveFunction : MonoBehaviour
{
    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(20,10);

    [SerializeField] private List<GridTile> tileSet;

    [SerializeField] private GridCell cellObject;

    private GraphManager _graphManager;
    private List<GridCell> gridCells = new List<GridCell>();

    [SerializeField] private GridTile startTile, endTile;

    private float timer = 0;
    [SerializeField] private bool auto = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_graphManager == null) { _graphManager = GetComponent<GraphManager>();}
        //if(ErrorCheck()){return;}
        InitialiseGrid();
        CollapseSetCells();
        CollapseCells();
        
        Debug.Log("IS VALID?: " + IsEndReachable());

    }

    void Update()
    {
        if(auto){timer = timer + Time.deltaTime;}
        
        if (Input.GetKeyUp(KeyCode.Space) || timer > 1f)
        {
            ScreenCapture.CaptureScreenshot("F:/GraphWFC/screenshot" + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") + ".png");;
            if (_graphManager == null) { _graphManager = GetComponent<GraphManager>();}
            //if(ErrorCheck()){return;}
            ResetCells();
            CollapseSetCells();
            CollapseCells();
            
            Debug.Log("IS VALID?: " + IsEndReachable());
            timer = 0;
        }
    }

    private void InitialiseGrid()
    {
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                GridCell newCell = Instantiate(cellObject, new Vector2(x, y), Quaternion.identity);
                newCell.transform.parent = this.transform;
                double[] distanceEdgeWeight = _graphManager.GetDistanceToClosestEdgeAndEdgeWeight(newCell.transform.position);
                newCell.CreateCellData(tileSet, false, distanceEdgeWeight[0]);
                newCell.SetClosestEdgeWeight(distanceEdgeWeight[1]);
                gridCells.Add(newCell);
            }
        }
    }
    
    private void ResetCells()
    {
        foreach (GridCell cell in gridCells)
        {
            cell.CreateCellData(tileSet, false, cell.distanceToEdge);
        }
    }

    private void CollapseSetCells()
    {
        if (startTile)
        {
            int index = GetClosestCellIndex(_graphManager.GetVertexWorldPosition(_graphManager.start));
            gridCells[index].SetTile(startTile);
            UpdateEntropy(index);
        }
        if (endTile)
        {
            int index = GetClosestCellIndex(_graphManager.GetVertexWorldPosition(_graphManager.end));
            gridCells[index].SetTile(endTile);
            UpdateEntropy(index);
        }
        
    }

    private void CollapseCells()
    {
        int index = 1;
        while (index != -1)
        {
            index = GetLowestEntropyCellIndex();
            if(index == -1){break;}
            gridCells[index].CollapseEntropy();
            UpdateEntropy(index);
            
        }
    }

    private int GetClosestCellIndex(Vector3 worldPos)
    {
        int closestIndex = 0;
        //Could be faster using a* distance heuristic but I am lazy
        for (int i = 0; i < gridCells.Count(); i ++)
        {
            if ((Vector3.Distance(gridCells[closestIndex].transform.position, worldPos) >
                 Vector3.Distance(gridCells[i].transform.position, worldPos)))
            {
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    private int GetLowestEntropyCellIndex()
    {
        int lowestEntropyIndex = -1;
        for(int i = 0; i < gridCells.Count; i++)
        {
            if(gridCells[i].GetCollapsed()){continue;} //Ignore if already collapsed

            if (lowestEntropyIndex == -1) { lowestEntropyIndex = i; continue;} //Set first to lowest entropy index

            if (gridCells[i].GetEntropy() < gridCells[lowestEntropyIndex].GetEntropy()) { lowestEntropyIndex = i; }
        }

        return lowestEntropyIndex;
    }

    private void UpdateEntropy(int centerIndex)
    {
        //up
        int i = centerIndex - gridDimensions.x;
        if( i > -1 && i < gridCells.Count){gridCells[i].ReducePossibleTilesByNeighbour(gridCells[centerIndex].GetAvailableNeighbours(1));}
        //down
        i = centerIndex + gridDimensions.x;
        if( i > -1 && i < gridCells.Count){gridCells[i].ReducePossibleTilesByNeighbour(gridCells[centerIndex].GetAvailableNeighbours(0));}
        //left
        i = centerIndex - 1;
        if( centerIndex % gridDimensions.x != 0){gridCells[i].ReducePossibleTilesByNeighbour(gridCells[centerIndex].GetAvailableNeighbours(2));}
        //right
        i = centerIndex + 1;
        if( (centerIndex + 1) % gridDimensions.x != 0){gridCells[i].ReducePossibleTilesByNeighbour(gridCells[centerIndex].GetAvailableNeighbours(3));}
    }

    private bool ErrorCheck()
    {
        if (gridDimensions.x < 1 || gridDimensions.y < 1)
        {
            Debug.LogError("Dimensions too small");
            return true;
        }
        if (tileSet.Count == 0)
        {
            Debug.LogError("No Tileset");
            return true;
        }

        return false;
    }

    /// <summary>
    /// This is a lazily implemented and cursed breadth first search. Horribly un-optimal but it technically works
    /// Should be an a* implementation
    /// Entirely my fault for the selected data structures
    /// </summary>
    /// <returns></returns>
    private bool IsEndReachable()
    {
        List<int> visited = new List<int>();
        Queue<GridCell> cellQueue = new Queue<GridCell>();
        cellQueue.Enqueue(gridCells[GetClosestCellIndex(_graphManager.GetVertexWorldPosition(_graphManager.start))]);

        GridCell endCell = gridCells[GetClosestCellIndex(_graphManager.GetVertexWorldPosition(_graphManager.end))];
        while (cellQueue.Count > 0)
        {
            if (cellQueue.Peek() == endCell) { return true; } //Is End?
            
            //Get current Index
            int centerIndex = -1;
            for (int i = 0; i < gridCells.Count; i++)
            {
                if (cellQueue.Peek().transform.position == gridCells[i].transform.position)
                {
                    centerIndex = i;
                    break;
                }
            }

            if (centerIndex < 0)
            {
                Debug.LogError("CELLNOTFOUND");
                return false;
            }

            //Check Neighbour Cells to add
            //up
            int index = centerIndex - gridDimensions.x;
            if( index > -1 && index < gridCells.Count){
                if (gridCells[index].tile.walkable && (!visited.Contains(index)))
                {
                    visited.Add(index);
                    if (gridCells[index] == endCell) { return true;}
                    cellQueue.Enqueue(gridCells[index]);
                }}
            //down
            index = centerIndex + gridDimensions.x;
            if (index > -1 && index < gridCells.Count)
            {
                if (gridCells[index].tile.walkable && (!visited.Contains(index)))
                {
                    visited.Add(index);
                    if (gridCells[index] == endCell) { return true;}
                    cellQueue.Enqueue(gridCells[index]);
                }
            }
            //left
            index = centerIndex - 1;
            if (centerIndex % gridDimensions.x != 0)
            {
                if (gridCells[index].tile.walkable && (!visited.Contains(index)))
                {
                    visited.Add(index);
                    if (gridCells[index] == endCell) { return true;}
                    cellQueue.Enqueue(gridCells[index]);
                }
            }
            //right
            index = centerIndex + 1;
            if ((centerIndex + 1) % gridDimensions.x != 0)
            {
                if (gridCells[index].tile.walkable && (!visited.Contains(index)))
                {
                    visited.Add(index);
                    if (gridCells[index] == endCell) { return true;}
                    cellQueue.Enqueue(gridCells[index]);
                }
            }
            visited.Add(centerIndex);
            cellQueue.Dequeue();
        }
        return false;
    }
}
