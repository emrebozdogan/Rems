namespace RemsAPI.DTOs;

public class SaveGeometriesRequestDto
{
    public required string PolygonA { get; set; }
    public required string PolygonB { get; set; }
    public required string PolygonC { get; set; }
}
