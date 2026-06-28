using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;
using Shared.DTOS.AuthDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("login")] // BaseUrl/api/auth/login
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var result = await _serviceManager.AuthService.LoginAsync(loginRequest);
            return Ok(result);
        }
        [HttpPost("register")] // BaseUrl/api/auth/register

        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var result = await _serviceManager.AuthService.RegisterAsync(registerRequest);
            return Ok(result);
        }


        //Check Email Exsist
        [HttpGet("emailexists")]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var result = await _serviceManager.AuthService.CheckEmailExsistAsync(email);
            return Ok(result);
        }

        //Get Current User
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.AuthService.GetCurrentUserAsync(email.Value);
            return Ok(result);
        }

        //Get Current User Address 
        [HttpGet("Address")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserAddress()
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.AuthService.GetCurrentUserAddressAsync(email.Value);
            return Ok(result);
        }

        //Update User Address 
        [HttpPut("Address")]
        [Authorize]
        public async Task<IActionResult> UpdateCurrentUserAddress(AddressDTO request)
        {
            var email = User.FindFirst(ClaimTypes.Email);
            var result = await _serviceManager.AuthService.UpdateCurrentUserAddressAsync(request,email.Value);
            return Ok(result);
        }
    }
}
