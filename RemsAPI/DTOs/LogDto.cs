namespace RemsAPI.DTOs;

public class LogDto
{
  public string? Id { get; set; }
  public string? UserId { get; set; }
  public string? UserName { get; set; }
  public string? Status { get; set; }
  public string? OperationType { get; set; }
  public string? Description { get; set; }
  public DateTime? Timestamp { get; set; }
  public string? IpAddress { get; set; }
}