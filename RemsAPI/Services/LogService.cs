using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class LogService(RemsDbContext context, IMapper mapper) : ILogService
{
  public async Task<PaginatedResults<LogDto>> GetLogsAsync(LogFilterDto logFilterDto)
  {
    var query = from l in context.Logs.AsNoTracking()
                join u in context.Users on l.UserId equals u.Id into userGroup
                from u in userGroup.DefaultIfEmpty()
                select new { Log = l, User = u };

    if (!string.IsNullOrEmpty(logFilterDto.UserName))
    {
      query = query.Where(x => x.User != null && EF.Functions.ILike(x.User.Name, $"%{logFilterDto.UserName}%"));
    }
    if (!string.IsNullOrEmpty(logFilterDto.Status))
    {
      query = query.Where(x => EF.Functions.ILike(x.Log.Status, $"%{logFilterDto.Status}%"));
    }
    if (!string.IsNullOrEmpty(logFilterDto.OperationType))
    {
      query = query.Where(x => EF.Functions.ILike(x.Log.OperationType, $"%{logFilterDto.OperationType}%"));
    }
    if (!string.IsNullOrEmpty(logFilterDto.Description))
    {
      query = query.Where(x => 
        (x.User != null && EF.Functions.ILike(x.Log.Description.Replace(x.Log.UserId, x.User.Name), $"%{logFilterDto.Description}%")) ||
        (x.User == null && EF.Functions.ILike(x.Log.Description, $"%{logFilterDto.Description}%"))
      );
    }
    if (logFilterDto.Timestamp != null)
    {
      var targetDate = DateTime.SpecifyKind(logFilterDto.Timestamp.Value.Date, DateTimeKind.Utc);
      query = query.Where(x => x.Log.Timestamp.Date == targetDate);
    }
    if (!string.IsNullOrEmpty(logFilterDto.IpAddress))
    {
      query = query.Where(x => EF.Functions.ILike(x.Log.IpAddress, $"%{logFilterDto.IpAddress}%"));
    }

    int totalCount = await query.CountAsync();

    var items = await query.OrderByDescending(x => x.Log.Timestamp)
                           .Skip((logFilterDto.PageNumber - 1) * logFilterDto.NumberOfLogs)
                           .Take(logFilterDto.NumberOfLogs)
                           .ToListAsync();

    var logs = new PaginatedResults<LogDto>
    {
      Data = items.Select(x => {
          var logDto = mapper.Map<LogDto>(x.Log);
          var userName = x.User != null ? x.User.Name : "Unknown";
          logDto.UserName = userName;
          if (logDto.Description != null && logDto.Description.Contains(x.Log.UserId)) {
              logDto.Description = logDto.Description.Replace(x.Log.UserId, userName);
          }
          return logDto;
      }).ToList(),
      TotalCount = totalCount,
      PageNumber = logFilterDto.PageNumber,
      PageSize = logFilterDto.NumberOfLogs
    };

    return logs;
  }
}
