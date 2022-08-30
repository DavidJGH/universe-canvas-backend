using Microsoft.Extensions.Options;
using MongoDB.Driver;
using universe_canvas.Models;

namespace universe_canvas.Utils;

public static class MyMongoUtils
{
    public static IMongoCollection<T> GetCollection<T>(IOptions<DatabaseSettings> databaseSettings,
        string collectionName)
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        return mongoDatabase.GetCollection<T>(collectionName);
    }
}