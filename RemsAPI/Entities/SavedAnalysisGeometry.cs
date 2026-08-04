using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace RemsAPI.Entities;

public class SavedAnalysisGeometry
{
    [Key]
    public required string UserId { get; set; }
    public required Geometry PolygonA { get; set; }
    public required Geometry PolygonB { get; set; }
    public required Geometry PolygonC { get; set; }
}
