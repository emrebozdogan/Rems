namespace RemsAPI.Entities;

public class Neighborhood
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string Name { get; set; }
  public required string DistrictId { get; set; }
  public District? District { get; set; }
}
