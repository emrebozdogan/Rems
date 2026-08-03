using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface ILogService
{
  public Task<PaginatedResults<LogDto>> GetLogsAsync(LogFilterDto logFilterDto);
  public Task<List<LogDto>> GetAllFilteredLogsAsync(LogFilterDto logFilterDto);
}
