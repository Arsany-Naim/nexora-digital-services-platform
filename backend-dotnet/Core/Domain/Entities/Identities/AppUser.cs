using Microsoft.AspNetCore.Identity;
using System.Net;

namespace Domain.Entities.Identities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
    }
}
