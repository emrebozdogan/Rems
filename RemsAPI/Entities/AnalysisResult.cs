using NetTopologySuite.Geometries;

namespace RemsAPI.Entities;

public class AnalysisResult
{
  public string Id { get; set; } = Guid.NewGuid().ToString();
  public required string UserId { get; set; }
  public required Geometry Geometry { get; set; }
  public double SurfaceArea { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
