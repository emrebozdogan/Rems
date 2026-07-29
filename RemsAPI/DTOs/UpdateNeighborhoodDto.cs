namespace RemsAPI.DTOs;

public class UpdateNeighborhoodDto
{
  public required string Id { get; set; }
  public required string Name { get; set; }
  public required string DistrictId { get; set; }
}