namespace Orders.Api.Models
{
    public class OrderItemModel
    {
        public Guid? ProductId { get; set; }

        public string? ProductName { get; set; }

        public int? Quantity { get; set; }
    }
}