namespace Basket.Api.Models
{
    public class BasketModel
    {
        public Guid? UserId { get; set; }
        public UserModel? User { get; set; }
        public IEnumerable<BasketProductItemModel>? Products { get; set; }
        public string City { get; set; } = string.Empty;

        public int ProductsTotalQuantity => Products?.Sum(p => p.Quantity) ?? 0;
    }
}