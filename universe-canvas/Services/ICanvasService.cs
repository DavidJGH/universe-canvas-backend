using universe_canvas.Models;

namespace universe_canvas.Services;

public interface ICanvasService
{
    public Canvas Canvas { get; set; }
    public PartialCanvas CanvasChanges { get; set; }
}