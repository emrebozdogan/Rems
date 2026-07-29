using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RemsAPI.Entities;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class TokenService(IConfiguration config) : ITokenService
{
  public string CreateToken(User user)
  {

    var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get the key");

    if (tokenKey.Length < 64) throw new Exception("Your token key needs to be >= 64 char");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

    var claims = new List<Claim>
    {
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.NameIdentifier, user.Id),
      new(ClaimTypes.Role, user.Role)
    };

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    var descriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.Now.AddDays(7),
      SigningCredentials = creds
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(descriptor);
    return tokenHandler.WriteToken(token);
  }
}
