using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions.BadRequest
{
    public class DeleteBasketBadRequestException() : BadRequestexception("Invalid Delete Basket Request")
    {
    }
}
