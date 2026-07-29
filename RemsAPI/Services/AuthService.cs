using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Exceptions;
using RemsAPI.Extensions;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class AuthService(RemsDbContext context, ITokenService tokenService) : IAuthService
{
  public async Task<UserDto> LoginAsync(UserLoginDto loginDto)
  {
    var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email) ?? throw new InvalidCredentialException("Invalid email or password");

    using var hmac = new HMACSHA256(user.PasswordSalt);

    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    for (int i = 0; i < computedHash.Length; i++)
    {
      if (computedHash[i] != user.PasswordHash[i])
      {
        throw new InvalidCredentialException("Invalid email or password");
      }
    }

    var token = tokenService.CreateToken(user);

    return user.ToDto(token);
  }

  public async Task<UserDto> RegisterAsync(UserRegisterDto registerDto)
  {
    if (await EmailExistsAsync(registerDto.Email))
    {
      throw new ConflictException("This email is already registered!");
    }

    using var hmac = new HMACSHA256();

    var newUser = new User
    {
      Name = registerDto.Name,
      Email = registerDto.Email,
      Role = "Regular",
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
      PasswordSalt = hmac.Key
    };

    try
    {
      context.Users.Add(newUser);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw new BadRequestException("Registration failed due to a database error.");
    }


    var token = tokenService.CreateToken(newUser);

    return newUser.ToDto(token);
  }

  private async Task<bool> EmailExistsAsync(string email)
  {
    return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
  }
}
