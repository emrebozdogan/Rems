using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.Exceptions;

namespace RemsAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BaseController : ControllerBase
  {
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedException("User ID not found in the current context.");
  }
}
