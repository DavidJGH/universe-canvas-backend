namespace universe_canvas.Models;

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string CanvasCollectionName { get; set; } = null!;
}