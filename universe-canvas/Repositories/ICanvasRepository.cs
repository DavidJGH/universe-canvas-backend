#nullable enable
using System.Threading.Tasks;
using universe_canvas.Models;

namespace universe_canvas.Repositories;

public interface ICanvasRepository
{
    Task<Canvas?> GetAsync();
    
    Task InsertAsync(Canvas newCanvas);
    
    Task ReplaceAsync(Canvas updatedCanvas);
}
