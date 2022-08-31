using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using universe_canvas.Models;
using universe_canvas.Services;

namespace universe_canvas.Hubs
{
    public class CanvasHub : Hub
    {
        private readonly ICanvasService _canvasService;

        public CanvasHub(ICanvasService canvasService)
        {
            _canvasService = canvasService;
        }

        public void SetPixel(int x, int y, int c)
        {
            _canvasService.Canvas.SetPixel(x, y, c);
            _canvasService.CanvasChanges.SetPixel(x, y, c);
        }

        public Canvas GetCanvas()
        {
            return _canvasService.Canvas;
        }
    }
}