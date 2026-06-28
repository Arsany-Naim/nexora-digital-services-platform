using Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations.ProductConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).HasColumnType("varchar").HasMaxLength(256);
            builder.Property(p => p.Description).HasColumnType("varchar").HasMaxLength(512);
            builder.Property(p => p.PictureUrl).HasColumnType("varchar").HasMaxLength(256);
            builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
            builder.HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(p => p.Type)
                .WithMany()
                .HasForeignKey(p => p.TypeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
