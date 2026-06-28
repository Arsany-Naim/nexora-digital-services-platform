using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.NotFound
{
    public class DeliveryMethodNotFoundException(int id) : NotFoundException($"Delivery method With Id {id} not found.")
    {
    }
}
