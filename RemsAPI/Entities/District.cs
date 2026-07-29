namespace RemsAPI.Entities;

public class District
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string Name { get; set; }
  public required string CityId { get; set; }
  public City? City { get; set; }
}
