using RemsAPI.Middlewares;

namespace RemsAPI.Extensions;

public static class LoggingMiddlewareExtension
{
  public static IApplicationBuilder UseCustomLogging(this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<LoggingMiddleware>();
  }
}
