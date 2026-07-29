using System.ComponentModel.DataAnnotations;

namespace RemsAPI.DTOs;

public class AreaAnalysisRequestDto
{
  [Required(ErrorMessage = "Please complete geometries A, B, and C.")]
  public required string PolygonA { get; set; }
  [Required(ErrorMessage = "Please complete geometries A, B, and C.")]
  public required string PolygonB { get; set; }
  [Required(ErrorMessage = "Please complete geometries A, B, and C.")]
  public required string PolygonC { get; set; }
  [Required(ErrorMessage = "Operation type is required.")]
  public required string OperationType { get; set; }
}
