#nullable enable
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace universe_canvas.Models
{
    public class Canvas
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Content { get; set; }
        public List<string> Palette { get; set; }
        public int StartColor { get; set; }

        public Canvas(int width, int height, int startColor, List<string> palette)
        {
            Width = width;
            Height = height;
            StartColor = startColor;
            Content = Enumerable.Repeat(startColor, width * height).ToArray();
            Palette = palette;
        }

        public void SetPixel(int x, int y, int c)
        {
            Content[y * Width + x] = c;
        }

        public void SetSize(int width, int height, bool forceSmaller)
        {
            if ((width < Width || height < Height) && !forceSmaller)
            {
                return;
            }
            var contentList = Content.ToList();
            if (height < Height)
            {
                contentList.RemoveRange(height * Width, (Height - height) * Width);
            }
            if (height > Height)
            {
                contentList.AddRange(Enumerable.Repeat(StartColor, (height - Height) * Width));
            }
            Height = height;
            if (width < Width)
            {
                for (int y = Height-1; y >= 0; y--)
                {
                    contentList.RemoveRange(y * Width + width, Width - width);
                }
            }
            if (width > Width)
            {
                for (int y = Height-1; y >= 0; y--)
                {
                    contentList.InsertRange(y * Width + Width, Enumerable.Repeat(StartColor, width - Width));
                }
            }
            Width = width;
            Content = contentList.ToArray();
        }
    }

    public class PartialCanvas
    {
        public List<PixelInfo> Content { get; set; }

        public PartialCanvas()
        {
            Content = new List<PixelInfo>();
        }

        public void SetPixel(int x, int y, int c)
        {
            Content.Add(new PixelInfo(x, y, c));
        }

        public void Clear()
        {
            Content.Clear();
        }
    }

    public class PixelInfo
    {
        public Vector Position { get; set; }
        public int ColorIndex { get; set; }

        public PixelInfo(int x, int y, int colorIndex)
        {
            Position = new Vector(x, y);
            ColorIndex = colorIndex;
        }
    }

    public class Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}