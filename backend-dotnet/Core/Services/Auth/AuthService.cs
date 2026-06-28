using Domain.Entities.Identities;
using Domain.Exceptions.BadRequest;
using Domain.Exceptions.NotFound;
using Domain.Exceptions.UnAuthorized;
using Microsoft.AspNetCore.Identity;
using Services.Abstractions.Auth;
using Shared.DTOS.AuthDTOS;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Services.Auth
{
    public class AuthService(UserManager<AppUser> _userManager,IOptions<JwtOptions> options,IMapper _mapper) : IAuthService
    {
        public async Task<bool> CheckEmailExsistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        public async Task<UserResponse?> GetCurrentUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                throw new UserNotFoundException(email);
            }
            return new UserResponse()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await GenerateTokenAsync(user)
            };
        }
        public async Task<AddressDTO?> GetCurrentUserAddressAsync(string email)
        {
            var user = await _userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user is null)
            {
                throw new UserNotFoundException(email);
            }
            return _mapper.Map<AddressDTO>(user.Address);
        }

        public async Task<AddressDTO?> UpdateCurrentUserAddressAsync(AddressDTO requaet, string email)
        {
            var user = await _userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user is null)
            {
                throw new UserNotFoundException(email);
            }
            if(user.Address is null)
            {
                //Create New Address
                user.Address = _mapper.Map<Address>(requaet);
            }
            else
            {
                //Update Address
                user.Address.FirstName = requaet.FirstName;
                user.Address.LastName = requaet.LastName;
                user.Address.Street = requaet.Street;
                user.Address.City = requaet.City;
                user.Address.Country = requaet.Country;
            }
            await _userManager.UpdateAsync(user);
            return _mapper.Map<AddressDTO>(user.Address);
        }

        public async Task<UserResponse?> LoginAsync(LoginRequest loginRequest)
        {
           var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null)
            {
                throw new UserNotFoundException(loginRequest.Email);
            }
            var flag = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!flag) throw new UnAuthorizedExcpetion();
            return new UserResponse()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await GenerateTokenAsync(user)
            };
        }

        public async Task<UserResponse?> RegisterAsync(RegisterRequest registerRequest)
        {
            var user = new AppUser()
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                DisplayName = registerRequest.DisplayName,
                PhoneNumber = registerRequest.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user,registerRequest.Password);
            if(!result.Succeeded)
            {
                throw new RegistrationBadRequestException(result.Errors.Select(E => E.Description).ToList());
            }
            return new UserResponse()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await GenerateTokenAsync(user)
            };
        }


        private async Task<string> GenerateTokenAsync(AppUser user)
        {
            //Token
            //1-Header       (type,algorithm)
            //2-Payloads     (Claims)
            //3-Segniture    (Key)

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,user.DisplayName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.MobilePhone,user.PhoneNumber)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var jwtOptions = options.Value;
            //KarasMiladIsTheBestinThisWorldKarasMiladIsTheBestinThisWorldKarasMiladIsTheBestinThisWorldKarasMiladIsTheBestinThisWorld
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience:jwtOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwtOptions.DurationInDayes),
                signingCredentials: new SigningCredentials(key,SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
