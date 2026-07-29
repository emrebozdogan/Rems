using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController(IUserService userService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetUsersForAdminAsync([FromQuery] UserFilterDto userFilterDto)
        {
            var result = await userService.GetUsersAsync(userFilterDto);
            return Ok(new { Message = "Users listed successfully.", Data = result });
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserByAdminAsync(CreateUserDto createUserDto)
        {
            var result = await userService.CreateUserAsync(createUserDto);
            return Ok(new { Message = "User added successfully.", Data = result });
        }
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> UpdateUserByAdminAsync(UpdateUserDto updateUserDto, string userId)
        {
            var result = await userService.UpdateUserAsync(updateUserDto, userId);
            return Ok(new { Message = "User updated successfully.", Data = result });
        }
        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteUserByAdminAsync(string userId)
        {
            var result = await userService.DeleteUserAsync(userId);
            return Ok(new { Message = "User and associated properties deleted successfully.", Data = result });
        }
    }
}
