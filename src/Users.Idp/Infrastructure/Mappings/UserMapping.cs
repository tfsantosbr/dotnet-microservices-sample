using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Idp.Domain;

namespace Users.Idp.Infrastructure.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users").HasKey(u => u.Id);
            builder.Property(u => u.Name).HasColumnType("varchar").HasMaxLength(500);
            builder.Property(u => u.Email).HasColumnType("varchar").HasMaxLength(500);
            builder.Property(u => u.Password).HasColumnType("varchar").HasMaxLength(200);
        }
    }
}