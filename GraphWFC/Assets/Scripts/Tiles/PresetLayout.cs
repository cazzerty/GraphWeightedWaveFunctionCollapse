using UnityEngine;

namespace Tiles
{
    public class PresetLayout : MonoBehaviour
    {
        public int[,] tilePattern = new int[5,5];
        // Start is called before the first frame update
        void Awake()
        {
            for (int x = 0; x < tilePattern.GetLength(0); x++)
            {
                for (int y = 0; y < tilePattern.GetLength(1); y++)
                {
                    if (x == 0 || x == tilePattern.GetLength(0) - 1 || y == 0 || y == tilePattern.GetLength(1) - 1)
                    {
                        tilePattern[x, y] = 1;
                    }
                    else
                    {
                        tilePattern[x, y] = 0;
                    }
                
                
                }
            }

            tilePattern[2, 0] = 0;
            tilePattern[2, 2] = 2;
        }

    }
}
