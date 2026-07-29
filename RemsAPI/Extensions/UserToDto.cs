using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Interfaces;

namespace RemsAPI.Extensions;

public static class UserToDto
{
  public static UserDto ToDto(this User user, string token)
  {
    return new UserDto
    {
      Id = user.Id,
      Email = user.Email,
      Name = user.Name,
      Token = token
    };
  }
}
