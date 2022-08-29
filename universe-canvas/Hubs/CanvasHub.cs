using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Models;

namespace universe_canvas.Hubs
{
    public class CanvasHub : Hub
    {
        public static Canvas Canvas { get; set; } = new(100, 100, 1, new List<string>
        {
            "#1a1c2c",
            "#f4f4f4",
            "#94b0c2",
            "#566c86",
            "#333c57",
            "#5d275d",
            "#b13e53",
            "#9c6137",
            "#ef7d57",
            "#ffcd75",
            "#a7f070",
            "#38b764",
            "#257179",
            "#3b5dc9",
            "#55cdfc",
            "#ffaec9",
        });

        public static PartialCanvas CanvasChanges = new();

        public void SetPixel(int x, int y, int c)
        {
            Canvas.SetPixel(x, y, c);
            CanvasChanges.SetPixel(x, y, c);
        }

        public Canvas GetCanvas()
        {
            return Canvas;
        }

        public static void ClearChanges()
        {
            CanvasChanges.Clear();
        }
    }
}