using Microsoft.AspNetCore.Diagnostics;
using RemsAPI.Exceptions;

namespace RemsAPI.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    var statusCode = exception switch
    {
      ConflictException => StatusCodes.Status409Conflict,
      NotFoundException => StatusCodes.Status404NotFound,
      UnauthorizedException => StatusCodes.Status401Unauthorized,
      _ => StatusCodes.Status500InternalServerError
    };

    httpContext.Response.StatusCode = statusCode;

    await httpContext.Response.WriteAsJsonAsync(new { detail = exception.Message });
    return true;
  }
}
