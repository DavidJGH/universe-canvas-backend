using System.Collections.Generic;
using System.Linq;

namespace universe_canvas.Models
{
    public class Canvas
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Content { get; set; }
        public List<string> Palette { get; set; }

        public Canvas(int width, int height, int startColor, List<string> palette)
        {
            Width = width;
            Height = height;
            Content = Enumerable.Repeat(startColor, width * height).ToArray();
            Palette = palette;
        }

        public void SetPixel(int x, int y, int c)
        {
            Content[y * Width + x] = c;
        }
    }
}