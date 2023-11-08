using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tiles;
using UnityEngine;
using UnityEngine.Serialization;
using Tile = UnityEngine.Tilemaps.Tile;

public class WaveFunction : MonoBehaviour
{
    [Header("GRAPH BASED WAVE FUNCTION COLLAPSE")]
    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(20,10);
    [SerializeField] private bool useGraphProximity = true;

    [Header("--Tile Setup--")]
    [SerializeField] private List<GridTile> tileSet;
    [SerializeField] private GridTile startTile, endTile;
    [FormerlySerializedAs("tileLayout")] [SerializeField] private PresetLayout presetLayout;
    [FormerlySerializedAs("patternTiles")] [SerializeField] private GridTile[] presetTiles = new GridTile[2];

    [Header("--Cell Setup--")]
    [SerializeField] private GridCell cellObject;
    private List<GridCell> gridCells = new List<GridCell>();
    [SerializeField] private int minimumWalkWeight = 1;
    [SerializeField] private int minimumNonWalkWeight = 0;
    
    [SerializeField, Range(0,2)] private int weightingTypeWalk = 1, weightingTypeNonWalk = 1;

    private GraphManager _graphManager;
    
    //SETTINGS
    private float timer = 0;
    [Header("DEBUG")]
    [SerializeField] private bool auto = false;



    private String SettingsString()
    {
        String s;
        return (s = $"{gridDimensions.x}x{gridDimensions.y}_GP-{useGraphProximity}_wTW-{weightingTypeWalk}_wTNW-{weightingTypeNonWalk}_mWW-{minimumWalkWeight}_mNWW-{minimumNonWalkWeight}");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (_graphManager == null) { _graphManager = GetComponent<GraphManager>();}
        //if(ErrorCheck()){return;}
        InitialiseGrid();
        SetCellValues();
        CollapsePresetCells();
        CollapseCells();
        Debug.Log(SettingsString());
        
        Debug.Log("IS VALID?: " + IsEndReachable());

    }

    void Update()
    {
        if(auto){timer = timer + Time.deltaTime;}

        if (Input.GetKeyUp(KeyCode.Return))
        {
            ScreenCapture.CaptureScreenshot("F:/GraphWFC/" + SettingsString() + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") + ".png");
        }
        
        if (Input.GetKeyUp(KeyCode.Space) || timer > 1f)
        {
            if (_graphManager == null) { _graphManager = GetComponent<GraphManager>();}
            //if(ErrorCheck()){return;}
            ResetCells();
            SetCellValues();
            CollapsePresetCells();
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
                newCell.name = "Cell_" + gridCells.Count.ToString();
            }
        }
    }
    
    /// <summary>
    /// Resets the graph to a state with entropy reset.
    /// </summary>
    private void ResetCells()
    {
        foreach (GridCell cell in gridCells)
        {
            cell.CreateCellData(tileSet, false, cell.distanceToEdge);
        }
    }
    
    private void SetCellValues()
    {
        foreach (GridCell cell in gridCells)
        {
            cell.minimumWalkWeight = minimumWalkWeight;
            cell.minimumNonWalkWeight = minimumNonWalkWeight;
            cell.weightingTypeWalk = weightingTypeWalk;
            cell.weightingTypeNonWalk = weightingTypeNonWalk;
        }
    }

    /// <summary>
    /// Collapses any preset cells such as the start and end.
    /// Tiles must be set in the inspector, otherwise that cell will not be set;
    /// </summary>
    private void CollapsePresetCells()
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
        
        //Used for adding a preset structure. Tiles must be added
        if (presetLayout)
        {
            foreach (Vertex vertex in _graphManager._vertices)
            {
                if (vertex.GetWeight() == 2)
                {
                    
                    int index = GetClosestCellIndex(vertex.GetWorldPosition());
                    index = index - gridDimensions.x * (presetLayout.tilePattern.GetLength(1) / 2);
                    index = index - (presetLayout.tilePattern.GetLength(0) / 2);
                    
                    for (int y = 0; y < presetLayout.tilePattern.GetLength(1); y++)
                    {
                        for (int x= 0; x < presetLayout.tilePattern.GetLength(0); x++)
                        {
                            gridCells[index].SetTile(presetTiles[presetLayout.tilePattern[x,y]]);
                            UpdateEntropy(index);
                            index++;
                        }

                        index = index - presetLayout.tilePattern.GetLength(0);

                        index = index + gridDimensions.x;
                    }
                    
                }
            }
        }
        
    }

    private void CollapseCells()
    {
        int index = 1;
        while (index != -1)
        {
            index = GetLowestEntropyCellIndex();
            if(index == -1){break;}
            gridCells[index].CollapseEntropy(useGraphProximity);
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
    ///
    ///
    /// FORGET ALL THAT I FIXED IT
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
