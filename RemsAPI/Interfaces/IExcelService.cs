using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IExcelService
{
  public Task<MemoryStream> ExportLogsToExcelAsync(List<LogDto> logs);
  public Task<MemoryStream> ExportPropertiesToExcelAsync(List<PropertyDto> properties);
}
