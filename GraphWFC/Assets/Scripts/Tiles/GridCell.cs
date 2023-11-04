using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tiles
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private bool collapsed;
        [SerializeField] private List<GridTile> availableTiles = new List<GridTile>();

        [SerializeField] private double distanceToEdge;
        private GridTile tile;
    
        public void CreateCellData(List<GridTile> tiles, bool collapsed)
        {
            availableTiles = tiles;
            this.collapsed = collapsed;
        }
        
        public void CreateCellData(List<GridTile> tiles, bool collapsed, double distance)
        {
            availableTiles = tiles;
            this.collapsed = collapsed;
            distanceToEdge = distance;
            //Debug.Log(distance);
        }

        public void CollapseEntropy()
        {
            int selector = 0;
            if (distanceToEdge > 1)
            {
                selector = 1;}
            
            //todo Weight probability by distance

            selector = Random.Range(0, GetEntropy());
            
            tile = Instantiate(availableTiles[selector], transform.position, Quaternion.identity);
            availableTiles = new List<GridTile>();
            tile.transform.parent = this.transform;
            collapsed = true;
        }

        public void SetPossibleTiles(List<GridTile> tiles)
        {
            this.availableTiles = tiles;
        }
        public void ReducePossibleTilesByNeighbour(List<GridTile> neighbourTiles)
        {
            List<GridTile> safeTiles = new List<GridTile>();
            foreach (var VARIABLE in neighbourTiles)
            {
                if(VARIABLE.name.Contains("Clone")){VARIABLE.name = VARIABLE.name.Replace("(Clone)","").Trim();}
            }
            //For some reason if a Grid Tile listed itself as a neighbour Unity would consider the neighbour as a clone, so I couldn't use .contains
            //Absolute pain and there definitely is a better way but this works and I am tired

            for (int i = 0; i < availableTiles.Count; i++)
            {
                for (int j = 0; j < neighbourTiles.Count; j++)
                {
                    if (neighbourTiles[j].name == availableTiles[i].name)
                    {
                        safeTiles.Add(availableTiles[i]);
                        break;
                    }
                }
                
            }
            
            
            availableTiles = safeTiles;
        }

        public int GetEntropy()
        {
            return availableTiles.Count;
        }

        /// <summary>
        /// 0_UP, 1,DOWN, 2_LEFT, 3_RIGHT
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public List<GridTile> GetAvailableNeighbours(int direction)
        {
            //Either return all possible neighbours of possible tiles
            //or return possible neighbours of current tile
            if (!collapsed)
            {
                Debug.Log("FFFFF");
                return new List<GridTile>();
            }
            switch (direction)
            {
                case 0:
                    return tile.upNeighbours;
                    break;
                case 1:
                    return tile.downNeighbours;
                    break;
                case 2:
                    return tile.leftNeighbours;
                    break;
                case 3:
                    return tile.rightNeighbours;
                    break;
                
                default:
                    Debug.LogError("INVALID DIR INPUT");
                    return new List<GridTile>();
                    break;
            }
            
        }

        public Vector3 GetCoordinates()
        {
            return transform.position;
        }

        public bool GetCollapsed() { return collapsed; }
    }
}
