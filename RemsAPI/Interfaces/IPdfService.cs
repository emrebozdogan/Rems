namespace RemsAPI.Interfaces;

public interface IPdfService
{
  public Task<MemoryStream> ExportLogsToPdfAsync();
}
