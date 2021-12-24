namespace Orders.Consumer.Models
{
    public class OrderItem
    {
        public Guid? ProductId { get; set; }

        public string? ProductName { get; set; }

        public int? Quantity { get; set; }
    }
}