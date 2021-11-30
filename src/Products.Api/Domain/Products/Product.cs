namespace Products.Api.Domain.Products
{
    public class Product
    {
        public Product(string name, decimal price, int quantity, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        private Product()
        {
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
    }
}