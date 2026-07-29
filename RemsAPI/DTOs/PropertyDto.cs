namespace RemsAPI.DTOs;

public class PropertyDto
{
  public required string Id { get; set; }
  public string? UserId { get; set; }
  public required string ParcelNumber { get; set; }
  public required string LotNumber { get; set; }
  public required string Address { get; set; }
  public required string Geometry { get; set; }
  public required string PropertyType { get; set; }
  public string? ImagePath { get; set; }
  public required string NeighborhoodName { get; set; }
  public required string DistrictName { get; set; }
  public required string CityName { get; set; }
  public required string NeighborhoodId { get; set; }
  public required string DistrictId { get; set; }
  public required string CityId { get; set; }
}
