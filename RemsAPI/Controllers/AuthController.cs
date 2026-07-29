using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    public class AuthController(IAuthService authService) : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto user)
        {
            var result = await authService.RegisterAsync(user);
            HttpContext.Items["RegisterUserId"] = result.Id;
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto user)
        {
            var result = await authService.LoginAsync(user);
            HttpContext.Items["LoginUserId"] = result.Id;
            return Ok(result);
        }
    }
}
