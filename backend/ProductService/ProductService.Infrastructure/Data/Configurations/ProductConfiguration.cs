using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;


namespace ProductService.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.OwnsOne(x => x.Price, priceBuilder =>
            {
                priceBuilder.Property(p => p.Cost)
                    .HasColumnName("Cost")
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                priceBuilder.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(3)
                    .HasDefaultValue(Currency.BYN);
            });

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.IsActive);
        }
    }
}