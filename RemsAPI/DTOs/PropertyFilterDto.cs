namespace RemsAPI.DTOs;

public class PropertyFilterDto
{
  public string? ParcelNumber { get; set; }
  public string? LotNumber { get; set; }
  public string? Address { get; set; }
  public string? PropertyType { get; set; }
  public string? NeighborhoodName { get; set; }
  public string? DistrictName { get; set; }
  public string? CityName { get; set; }
  public string? OwnerId { get; set; }
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}
