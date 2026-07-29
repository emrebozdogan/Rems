using NetTopologySuite.Geometries;

namespace RemsAPI.Entities;

public class Property
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string ParcelNumber { get; set; }
  public required string LotNumber { get; set; }
  public required string Address { get; set; }
  public required Geometry Geometry { get; set; }
  public required string PropertyType { get; set; }
  public string? ImagePath { get; set; }
  public required string NeighborhoodId { get; set; }
  public Neighborhood? Neighborhood { get; set; }
  public string? UserId { get; set; }
  public User? User { get; set; }
}
