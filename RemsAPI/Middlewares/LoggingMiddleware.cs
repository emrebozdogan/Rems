using System.Security.Claims;
using RemsAPI.Data;
using RemsAPI.Entities;

namespace RemsAPI.Middlewares;

public class LoggingMiddleware(RequestDelegate next)
{
  public async Task InvokeAsync(HttpContext httpContext, RemsDbContext context)
  {
    var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
    var path = httpContext.Request.Path.Value ?? "none";
    var method = httpContext.Request.Method;
    var basicPath = path.Split("/")[^1];

    var segments = path.Split("/", StringSplitOptions.RemoveEmptyEntries);
    var target = "Unknown";
    if (segments.Length > 0)
    {
      var meaningfulSegments = segments.Where(s => s.ToLower() != "api" && !Guid.TryParse(s, out _)).ToList();
      if (meaningfulSegments.Any())
      {
        target = string.Join(" ", meaningfulSegments);
      }
    }

    method = method switch
    {
      "GET" => "View",
      "POST" => "Create",
      "PUT" => "Update",
      "DELETE" => "Delete",
      _ => "None",
    };
    var operationType = $"{method} {target}";

    if (basicPath.ToLower() == "login")
    {
      operationType = "Login";
    }
    else if (basicPath.ToLower() == "excel")
    {
      operationType = "Export Excel";
    }
    var log = new Log
    {
      UserId = userId,
      IpAddress = httpContext.Connection.RemoteIpAddress!.ToString(),
      OperationType = operationType,
      Timestamp = DateTime.UtcNow
    };

    try
    {
      await next(httpContext);
    }
    finally
    {
      log.Status = httpContext.Response.StatusCode.ToString();
      if (int.TryParse(log.Status, out int statusCode))
      {
        if (statusCode >= 200 && statusCode < 300)
        {
          log.Status = "Succeed";
        }
        else
        {
          log.Status = "Failed";
        }
      }

      if (httpContext.Items["LoginUserId"] is string loginUserId)
      {
        log.UserId = loginUserId;
      }

      if (httpContext.Items["RegisterUserId"] is string registerUserId)
      {
        log.UserId = registerUserId;
      }

      log.Description = $"User {log.UserId} performed {log.OperationType} operation. (Status: {log.Status})";

      context.Logs.Add(log);
      await context.SaveChangesAsync();
    }
  }
}
