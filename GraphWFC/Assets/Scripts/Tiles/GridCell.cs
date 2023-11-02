using System.Collections.Generic;
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
            Debug.Log(distance);
        }

        public void CollapseEntropy()
        {
            int selector = 0;
            if (distanceToEdge > 3)
            {
                selector = 1;}
            tile = Instantiate(availableTiles[selector], transform.position, Quaternion.identity);
            tile.transform.parent = this.transform;
        }

        public void SetTiles(List<GridTile> tiles)
        {
            this.availableTiles = tiles;
        }

        public int GetEntropy()
        {
            return availableTiles.Count;
        }
    }
}
