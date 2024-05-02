namespace Orders.Api.Models
{
    public class OrderModel
    {
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public double? ConfirmationTimeInHours
        {
            get
            {
                if (!ConfirmedAt.HasValue)
                    return null;

                var duration = ConfirmedAt - CreatedAt;
                
                return Math.Round(duration.Value.TotalHours, 2);
            }
        }

        public IEnumerable<OrderItemModel>? Products { get; set; }

        public void Confirm()
        {
            ConfirmedAt = DateTime.UtcNow;
        }
    }
}