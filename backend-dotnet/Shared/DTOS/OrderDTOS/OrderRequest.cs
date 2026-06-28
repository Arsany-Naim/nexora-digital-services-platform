using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.OrderDTOS
{
    public class OrderRequest
    {
        public string BasketId { get; set; }
        public int DeliveryMethodId { get; set; }
        public OrderAddressDTO ShipToAddress { get; set; }
    }
}
