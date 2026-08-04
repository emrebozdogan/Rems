using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Exceptions;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class UserService(RemsDbContext context, IMapper mapper) : IUserService
{
  public async Task<UserViewDto> CreateUserAsync(CreateUserDto createUserDto)
  {
    if (await EmailExistsAsync(createUserDto.Email))
    {
      throw new ConflictException("This email is already registered!");
    }

    using var hmac = new HMACSHA256();

    var newUser = mapper.Map<User>(createUserDto);

    newUser.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(createUserDto.Password));
    newUser.PasswordSalt = hmac.Key;

    try
    {
      context.Users.Add(newUser);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {

      throw new BadRequestException("Creating user failed due to a database error.");
    }
    return mapper.Map<UserViewDto>(newUser);
  }

  public async Task<bool> DeleteUserAsync(string userId)
  {
    var user = await context.Users.FindAsync(userId);

    if (user != null && !user.IsDeleted)
    {
      // making changes in multiple tables in database so db can be consistent
      await using var transaction = await context.Database.BeginTransactionAsync();
      try
      {
        var userProperties = await context.Properties.Where(p => p.UserId == user.Id).ToListAsync();
        foreach (var userProperty in userProperties)
        {
          userProperty.UserId = null;
          userProperty.User = null;
        }
        user.IsDeleted = true;
        await context.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      catch (Exception)
      {
        await transaction.RollbackAsync(); // rollback when error happens.
        throw new BadRequestException("Deleting user failed due to a database error.");
      }
      return true;
    }
    return false;
  }

  public async Task<PaginatedResults<UserViewDto>> GetUsersAsync(UserFilterDto userFilterDto)
  {
    var query = context.Users.AsNoTracking().AsQueryable().Where(u => u.IsDeleted == false);
    if (!string.IsNullOrEmpty(userFilterDto.Id))
    {
      query = query.Where(u => u.Id == userFilterDto.Id);
    }
    if (!string.IsNullOrEmpty(userFilterDto.Name))
    {
      query = query.Where(u => u.Name.ToLower().Contains(userFilterDto.Name.ToLower()));
    }
    if (!string.IsNullOrEmpty(userFilterDto.Email))
    {
      query = query.Where(u => u.Email.ToLower().Contains(userFilterDto.Email.ToLower()));
    }
    if (!string.IsNullOrEmpty(userFilterDto.Role))
    {
      query = query.Where(u => u.Role.ToLower().Contains(userFilterDto.Role.ToLower()));
    }

    var totalCount = await query.CountAsync();
    var items = await query.Skip((userFilterDto.PageNumber - 1) * userFilterDto.PageSize).Take(userFilterDto.PageSize).ToListAsync();

    var users = new PaginatedResults<UserViewDto>
    {
      Data = mapper.Map<List<UserViewDto>>(items),
      TotalCount = totalCount,
      PageSize = userFilterDto.PageSize,
      PageNumber = userFilterDto.PageNumber,
    };

    return users;
  }

  public async Task<UserViewDto> UpdateUserAsync(UpdateUserDto updateUserDto, string userId)
  {
    var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);

    if (user != null)
    {
      if (user.Email.ToLower() != updateUserDto.Email.ToLower() && await EmailExistsAsync(updateUserDto.Email))
      {
        throw new ConflictException("This email is already registered!");
      }

      user.Email = updateUserDto.Email;
      user.Name = updateUserDto.Name;
      user.Role = updateUserDto.Role;

      if (!string.IsNullOrEmpty(updateUserDto.Password))
      {
        using var hmac = new HMACSHA256();

        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(updateUserDto.Password));
        user.PasswordSalt = hmac.Key;
      }

      try
      {
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("Updating user failed due to a database error.");
      }

      return mapper.Map<UserViewDto>(user);
    }

    throw new NotFoundException("This user does not exist.");
  }

  private async Task<bool> EmailExistsAsync(string email)
  {
    return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
  }
}
