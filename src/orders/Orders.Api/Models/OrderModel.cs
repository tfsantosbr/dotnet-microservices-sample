namespace Orders.Api.Models
{
    public class OrderModel
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public IEnumerable<OrderItemModel>? Products { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Confirmed
    }
    
}