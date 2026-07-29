namespace RemsAPI.Entities;

public class City
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string Name { get; set; }
}
