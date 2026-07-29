namespace RemsAPI.Interfaces;

public interface IExcelService
{
  public Task<MemoryStream> ExportLogsToExcelAsync();
}
