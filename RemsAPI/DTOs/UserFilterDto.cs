namespace RemsAPI.DTOs;

public class UserFilterDto
{
  public string? Id { get; set; }
  public string? Name { get; set; }
  public string? Email { get; set; }
  public string? Role { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 50;
}
