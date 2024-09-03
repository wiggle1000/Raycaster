using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Raycaster
{
    internal class MapLayer
    {

        public int width, height = 32;
        public int[,] layer;

        public MapLayer(int w, int h)
        {
            width = w;
            height = h;
            layer = new int[width, height];
        }

        public bool isInBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public void SetAt(int x, int y, int setTo)
        {
            if (!isInBounds(x, y)) return;
            layer[x, y] = setTo;
        }

        public int GetAt(int x, int y)
        {
            if (!isInBounds(x, y)) return 0;
            return layer[x, y];
        }
    }
}
