namespace RemsAPI.DTOs;

public class AreaAnalysisResponseDto
{
  public string? ResultGeometry { get; set; }
  public double SurfaceArea { get; set; }
  public required string Message { get; set; }
}
