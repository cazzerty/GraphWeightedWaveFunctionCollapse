using System.Collections;
using System.Collections.Generic;
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
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (_graphManager == null) { _graphManager = GetComponent<GraphManager>();}
        //if(ErrorCheck()){return;}
        InitialiseGrid();
        CollapseCells();

    }

    private void InitialiseGrid()
    {
        for (int y = 0; y < gridDimensions.y; y++)
        {
            for (int x = 0; x < gridDimensions.x; x++)
            {
                GridCell newCell = Instantiate(cellObject, new Vector2(x, y), Quaternion.identity);
                newCell.transform.parent = this.transform;
                newCell.CreateCellData(tileSet, false, _graphManager.GetDistanceToClosestEdge(newCell.transform.position));
                gridCells.Add(newCell);
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
            gridCells[index].CollapseEntropy();
            UpdateEntropy(index);
            
        }
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
}
