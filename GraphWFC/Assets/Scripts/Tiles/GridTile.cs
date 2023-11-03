using System.Collections.Generic;
using UnityEngine;

namespace Tiles
{
    public class GridTile : MonoBehaviour
    {
        public List<GridTile> upNeighbours;
        public List<GridTile> downNeighbours;
        public List<GridTile> leftNeighbours;
        public List<GridTile> rightNeighbours;
        public double weight = 1;
        public bool walkable = true;
        public int limit;
    }
}
