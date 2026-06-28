using Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations.OrderConfigurations
{
    public class DeliveryMethodConfigurations : IEntityTypeConfiguration<DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
            builder.Property(DM => DM.ShortName).HasColumnType("varchar").HasMaxLength(128);
            builder.Property(DM => DM.Description).HasColumnType("varchar").HasMaxLength(256);
            builder.Property(DM => DM.DeliveryTime).HasColumnType("varchar").HasMaxLength(128);
            builder.Property(DM => DM.Price).HasColumnType("decimal(18,2)");
        }
    }
}
