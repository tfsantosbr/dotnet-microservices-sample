using Elastic.Apm.Api;
using Elastic.Apm.MongoDb;
using MongoDB.Driver;
using Orders.Consumer.Models;

namespace Orders.Consumer.Repositories
{
    public class OrderRepository
    {
        private readonly IMongoCollection<Order> _booksCollection;

        public OrderRepository(IConfiguration configuration, ITracer tracer)
        {
            var mongoConnectionString = configuration.GetSection("OrdersDatabase:ConnectionString").Value;
            var mongoSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);
            mongoSettings.ClusterConfigurator = builder => builder.Subscribe(new MongoDbEventSubscriber());

            var mongoClient = new MongoClient(mongoSettings);

            var mongoDatabase = mongoClient.GetDatabase(
                configuration.GetSection("OrdersDatabase:DatabaseName").Value);

            _booksCollection = mongoDatabase.GetCollection<Order>(
                configuration.GetSection("OrdersDatabase:OrdersCollectionName").Value);
        }

        public async Task CreateAsync(Order newOrder)
        {
            await _booksCollection.InsertOneAsync(newOrder);
        }
    }
}