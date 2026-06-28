using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations.OrderConfigurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress);
            builder.HasOne(o=> o.DeliveryMethod).
                WithMany().
                HasForeignKey(o=> o.DeliveryMethodId);
            builder.HasMany(O => O.Items).
                WithOne().
                OnDelete(DeleteBehavior.Cascade);
            builder.Property(o => o.SubTotal).HasColumnType("decimal(18,2)");
        }
    }
}
