using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IPdfService
{
  public Task<MemoryStream> ExportLogsToPdfAsync(List<LogDto> logs);
  public Task<MemoryStream> ExportPropertiesToPdfAsync(List<PropertyDto> properties);
}
