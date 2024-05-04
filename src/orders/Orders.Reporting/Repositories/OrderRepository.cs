using MongoDB.Bson;
using MongoDB.Driver;
using Orders.Reporting.Models;

namespace Orders.Reporting.Repositories;

public class OrderRepository
{
    private readonly IMongoCollection<Order> _booksCollection;

    public OrderRepository(IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetSection("OrdersDatabase:ConnectionString").Value;
        var mongoSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);

        var mongoClient = new MongoClient(mongoSettings);

        var mongoDatabase = mongoClient.GetDatabase(
            configuration.GetSection("OrdersDatabase:DatabaseName").Value);

        _booksCollection = mongoDatabase.GetCollection<Order>(
            configuration.GetSection("OrdersDatabase:OrdersCollectionName").Value);
    }

    public async Task<long> CountPendingOrders()
    {
        return await _booksCollection.CountDocumentsAsync(x => x.Status == OrderStatus.Pending);
    }

    internal async Task<long> CountConfirmedOrders()
    {
        return await _booksCollection.CountDocumentsAsync(x => x.Status == OrderStatus.Confirmed);
    }

    internal async Task<long> CountTotalOrders()
    {
        return await _booksCollection.CountDocumentsAsync(new BsonDocument());
    }
}