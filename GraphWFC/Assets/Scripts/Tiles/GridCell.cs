using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tiles
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private bool collapsed;
        [SerializeField] private List<GridTile> availableTiles = new List<GridTile>();

        [SerializeField] public double distanceToEdge;
        [SerializeField] private double closestEdgeWeight = 4;
        public GridTile tile;
    
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
            if(tile){Destroy(tile.gameObject);}
            //Debug.Log(distance);
        }

        public void CollapseEntropy()
        {
            int selector = 0;
            if (distanceToEdge > 1)
            {
                selector = 1;}
            
            //todo Weight probability by distance
            selector = WeightedRandomSelection();
            
            Console.WriteLine(this.transform.position);

            tile = Instantiate(availableTiles[selector], transform.position, Quaternion.identity); 
            availableTiles = new List<GridTile>();
            tile.transform.parent = this.transform;
            collapsed = true;
        }

        private int WeightedRandomSelection()
        {
            int ongoingCount = 0;
            int[] probabilityArray = new int[availableTiles.Count];
            for (int i = 0; i < availableTiles.Count; i++)
            {
                ongoingCount = ongoingCount + WeightFunction(availableTiles[i]);
                probabilityArray[i] = ongoingCount;
                
            }
            int selector = Random.Range(0, ongoingCount);
            for (int i = 0; i < probabilityArray.Length; i++)
            {
                if (selector < probabilityArray[i])
                {
                    return i;
                }
            }

            return 0;

        }

        private int WeightFunction(GridTile gridTile)
        {
            //return gridTile.weight;
            //return gridTile.weight;
            if (gridTile.walkable)
            {
                double weightedProb = System.Math.Pow(gridTile.weight, closestEdgeWeight - distanceToEdge);
                //double weightedProb = gridTile.weight * closestEdgeWeight - distanceToEdge;
                if (weightedProb < 1) { return 1;} //minimum
                //Debug.Log(weightedProb);
                return (int)Math.Round(weightedProb);
            }
            
            //return gridTile.weight;

            if (!gridTile.walkable)
            {
                double weightedProb = System.Math.Pow(gridTile.weight, distanceToEdge - closestEdgeWeight);
                //double weightedProb = gridTile.weight * distanceToEdge - closestEdgeWeight;
                if (weightedProb < 1) { return 0;} //minimum

                if (weightedProb > 65536)
                {
                    return 65536;}
                //Debug.Log(weightedProb);
                return (int)Math.Round(weightedProb);
            }
            return gridTile.weight;
        }

        public void SetTile(GridTile gridTile)
        {
            tile = Instantiate(gridTile, transform.position, Quaternion.identity);
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
            //return availableTiles.Count;
            return availableTiles.Count + ((int)distanceToEdge)/100;
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
                return new List<GridTile>();
            }
            switch (direction)
            {
                case 0:
                    return tile.upNeighbours;
                case 1:
                    return tile.downNeighbours;
                case 2:
                    return tile.leftNeighbours;
                case 3:
                    return tile.rightNeighbours;
                default:
                    Debug.LogError("INVALID DIR INPUT");
                    return new List<GridTile>();
            }
            
        }

        public Vector3 GetCoordinates()
        {
            return transform.position;
        }

        public bool GetCollapsed() { return collapsed; }

        public void SetClosestEdgeWeight(double edgeWeight)
        {
            closestEdgeWeight = edgeWeight;
        }
    }
}
