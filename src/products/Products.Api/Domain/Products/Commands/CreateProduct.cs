namespace Products.Api.Domain.Products.Commands
{
    public class CreateProduct
    {
        public String Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}