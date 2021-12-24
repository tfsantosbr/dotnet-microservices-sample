using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Api.Domain.Products;

namespace Products.Api.Infrastructure.Mappings
{
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products").HasKey(p => p.Id);
            builder.Property(p => p.Name).HasColumnType("varchar").HasMaxLength(500);
            builder.Property(p => p.Price).HasPrecision(14, 2);
        }
    }
}