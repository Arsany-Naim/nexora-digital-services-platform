using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOS.AuthDTOS
{
    public class UserResponse
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
