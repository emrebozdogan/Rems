using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IUserService
{
  public Task<PaginatedResults<UserViewDto>> GetUsersAsync(UserFilterDto userFilterDto);
  public Task<UserViewDto> CreateUserAsync(CreateUserDto createUserDto);
  public Task<UserViewDto> UpdateUserAsync(UpdateUserDto updateUserDto, string userId);
  public Task<bool> DeleteUserAsync(string userId);
}
