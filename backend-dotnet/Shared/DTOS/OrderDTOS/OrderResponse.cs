using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.OrderDTOS
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; }
        public string Status { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public string DeliveryMethod { get; set; } //DeliveryMethod Name
        public OrderAddressDTO ShippingAddress { get; set; }
        public ICollection<OrderItemDTO> Items { get; set; }
        public decimal SubTotal { get; set; } // ProductPrice * Quantity
        public decimal Total { get; set; } 
    }
}
