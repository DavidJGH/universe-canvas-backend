#nullable enable
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using universe_canvas.Models;
using universe_canvas.Utils;

namespace universe_canvas.Repositories;

public class CanvasRepository : ICanvasRepository
{
    private readonly IMongoCollection<Canvas> _canvasCollection;

    public CanvasRepository(
        IOptions<DatabaseSettings> databaseSettings)
    {
        _canvasCollection = MyMongoUtils.GetCollection<Canvas>(databaseSettings,
            databaseSettings.Value.CanvasCollectionName);
    }
    
    public async Task<Canvas?> GetAsync() =>
        await _canvasCollection.Find(_ => true).FirstOrDefaultAsync();
    
    public async Task InsertAsync(Canvas newCanvas) =>
        await _canvasCollection.InsertOneAsync(newCanvas);

    public async Task ReplaceAsync(Canvas updatedCanvas) =>
        await _canvasCollection.ReplaceOneAsync(_ => true, updatedCanvas);
}