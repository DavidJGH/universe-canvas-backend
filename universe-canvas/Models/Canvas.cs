namespace universe_canvas.Models
{
    public class Canvas
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Content { get; set; }

        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;
            Content = new int[width * height];
        }

        public void SetPixel(int x, int y, int c)
        {
            Content[y * Width + x] = c;
        }
    }
}