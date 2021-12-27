using Elastic.Apm.Api;
using MongoDB.Driver;
using Orders.Consumer.Models;

namespace Orders.Consumer.Repositories
{
    public class OrderRepository
    {
        private readonly IMongoCollection<Order> _booksCollection;
        private readonly ITracer _tracer;

        public OrderRepository(IConfiguration configuration, ITracer tracer)
        {
            var mongoClient = new MongoClient(
                configuration.GetSection("OrdersDatabase:ConnectionString").Value);

            var mongoDatabase = mongoClient.GetDatabase(
                configuration.GetSection("OrdersDatabase:DatabaseName").Value);

            _booksCollection = mongoDatabase.GetCollection<Order>(
                configuration.GetSection("OrdersDatabase:OrdersCollectionName").Value);
            _tracer = tracer;
        }

        public async Task CreateAsync(Order newOrder)
        {
            var saveOrderSpan = _tracer.CurrentTransaction.StartSpan("Save Order", "Order Transaction");

            try
            {
                await _booksCollection.InsertOneAsync(newOrder);
            }
            catch (Exception exception)
            {
                saveOrderSpan.CaptureException(exception);
            }
            finally
            {
                saveOrderSpan.End();
            }
        }
    }
}