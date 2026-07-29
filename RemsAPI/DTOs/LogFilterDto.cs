namespace RemsAPI.DTOs;

public class LogFilterDto
{
  public string? UserName { get; set; }
  public string? Status { get; set; }
  public string? OperationType { get; set; }
  public string? Description { get; set; }
  public DateTime? Timestamp { get; set; }
  public string? IpAddress { get; set; }
  public int PageNumber { get; set; } = 1;
  public int NumberOfLogs { get; set; } = 10;
}
