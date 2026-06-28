using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Product
{
    public class ProductType : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}
