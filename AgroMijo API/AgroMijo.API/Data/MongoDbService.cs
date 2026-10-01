using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AgroMijo.API.Data;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);

        _database = client.GetDatabase(
            settings.Value.DatabaseName
        );
    }

    public IMongoDatabase Database => _database;
}