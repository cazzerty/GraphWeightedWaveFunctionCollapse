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
    }

    // Update is called once per frame
    void Update()
    {
        
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
