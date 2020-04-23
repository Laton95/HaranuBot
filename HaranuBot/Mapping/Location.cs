using System;
using System.Collections.Generic;
using System.Text;

namespace HaranuBot.Mapping
{
    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string Detail { get; set; }
        public string Map { get; set; }

        public Location(int x, int y, int height, int width, string detail, string map)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
            Detail = detail;
            Map = map;
        }
    }
}
