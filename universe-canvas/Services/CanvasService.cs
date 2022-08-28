#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using universe_canvas.Models;

namespace universe_canvas.Services;

public class CanvasService
{
    private readonly IMongoCollection<Canvas> _canvasCollection;

    public CanvasService(
        IOptions<DatabaseSettings> databaseSettings)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _canvasCollection = mongoDatabase.GetCollection<Canvas>(
            databaseSettings.Value.CanvasCollectionName);
    }
    
    public async Task InsertAsync(Canvas updatedCanvas) =>
        await _canvasCollection.InsertOneAsync(updatedCanvas);

    public async Task<Canvas?> GetAsync() =>
        await _canvasCollection.Find(_ => true).FirstOrDefaultAsync();
    
    public async Task<ReplaceOneResult> ReplaceAsync(Canvas updatedCanvas) =>
        await _canvasCollection.ReplaceOneAsync(_ => true, updatedCanvas);
}