using MongoDB.Driver;
using Orders.Api.Models;

namespace Orders.Api.Repositories;

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

        public async Task<Order?> GetAsync(Guid orderId)
        {
            var order = await _booksCollection.FindAsync(x => x.OrderId == orderId);
            return await order.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            await _booksCollection.ReplaceOneAsync(x => x.OrderId == order.OrderId, order);
        }
    }