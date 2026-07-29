using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IAuthService
{
  Task<UserDto> RegisterAsync(UserRegisterDto registerDto);
  Task<UserDto> LoginAsync(UserLoginDto loginDto);
}
