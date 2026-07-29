namespace RemsAPI.Entities;

public class Log
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string UserId { get; set; }
  public string? Status { get; set; }
  public required string OperationType { get; set; }
  public string? Description { get; set; }
  public required DateTime Timestamp { get; set; }
  public required string IpAddress { get; set; }
}
