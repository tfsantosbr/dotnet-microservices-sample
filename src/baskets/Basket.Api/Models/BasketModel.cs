namespace Basket.Api.Models
{
    public class BasketModel
    {
        public Guid? UserId { get; set; }
        public UserModel? User { get; set; }
        public IEnumerable<BasketProductItemModel>? Products { get; set; }
    }
}