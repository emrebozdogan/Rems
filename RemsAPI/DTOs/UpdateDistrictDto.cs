namespace RemsAPI.DTOs;

public class UpdateDistrictDto
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string CityId { get; set; }
}