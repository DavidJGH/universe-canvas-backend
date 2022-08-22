using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Models;

namespace universe_canvas.Hubs
{
    public class CanvasHub : Hub
    {
        public static Canvas Canvas { get; set; } = new Canvas(100, 100);
        
        public void SetPixel(int x, int y, int c)
        {
            Canvas.SetPixel(x, y, c);
        }

        public Canvas GetCanvas()
        {
            return Canvas;
        }
    }
}