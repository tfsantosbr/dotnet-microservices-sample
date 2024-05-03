using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Orders.Consumer.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        public IEnumerable<OrderItem>? Products { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed
    }
}