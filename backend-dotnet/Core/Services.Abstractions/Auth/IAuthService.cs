using Shared.DTOS.AuthDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions.Auth
{
    public interface IAuthService
    {
        Task<UserResponse?> LoginAsync(LoginRequest loginRequest);
        Task<UserResponse?> RegisterAsync(RegisterRequest registerRequest);
        Task<bool> CheckEmailExsistAsync(string email);
        Task<UserResponse?> GetCurrentUserAsync(string email);
        Task<AddressDTO?> GetCurrentUserAddressAsync(string email);
        Task<AddressDTO?> UpdateCurrentUserAddressAsync(AddressDTO requaet,string email);

    }
}
