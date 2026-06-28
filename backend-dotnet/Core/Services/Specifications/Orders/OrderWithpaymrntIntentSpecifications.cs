using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.Orders
{
    public class OrderWithpaymrntIntentSpecifications : BaseSpecifications<Guid,Domain.Entities.Orders.Order>
    {
        public OrderWithpaymrntIntentSpecifications(string PaymentIntentId) : base(o => o.PaymentIntentId == PaymentIntentId)
        {
            
        }
    }
}
