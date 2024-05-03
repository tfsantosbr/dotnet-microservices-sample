using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Orders.Api.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public OrderStatus Status { get; set; }
    public IEnumerable<OrderItem>? Products { get; set; }

    public void Confirm()
    {
        ConfirmedAt = DateTime.UtcNow;
        Status = OrderStatus.Confirmed;
    }
}

public class OrderItem
{
    public Guid? ProductId { get; set; }

    public string? ProductName { get; set; }

    public int? Quantity { get; set; }
}

