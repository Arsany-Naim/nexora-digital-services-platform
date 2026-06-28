using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Specifications.Orders
{
    public class OrderSpecifications : BaseSpecifications<Guid, Domain.Entities.Orders.Order>
    {
        public OrderSpecifications(Guid id , string userEmail) : base(o => o.Id == id && o.UserEmail.ToLower() == userEmail.ToLower())
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
        }
        public OrderSpecifications(string userEmail) : base(o => o.UserEmail.ToLower() == userEmail.ToLower())
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
            AddOrderByDescending(o => o.OrderDate);
        }
    }
}
