
using System.Collections.Generic;
using universe_canvas.Models;

namespace universe_canvas.Services;

public class CanvasService : ICanvasService
{
    public Canvas Canvas { get; set; } = new(200, 200, 1, new List<string>
    {
        "#1a1c2c",
        "#f4f4f4",
        "#94b0c2",
        "#566c86",
        "#333c57",
        "#5d275d",
        "#b13e53",
        "#ef7d57",
        "#ffcd75",
        "#a7f070",
        "#38b764",
        "#257179",
        "#3b5dc9",
        "#55cdfc",
        "#ffaec9",
        "#9c6137",
    });

    public PartialCanvas CanvasChanges { get; set; } = new();
}