namespace Basket.Api.Models
{
    public class BasketProductItemModel
    {
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public int? Quantity { get; set; }
    }

    public enum ProductCategory
    {
        Electronics,
        Books,
        Food
    }
}