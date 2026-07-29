using RemsAPI.Entities;

namespace RemsAPI.Interfaces;

public interface ITokenService
{
  string CreateToken(User user);
}
