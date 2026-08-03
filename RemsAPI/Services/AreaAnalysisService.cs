using NetTopologySuite.IO;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Exceptions;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class AreaAnalysisService(RemsDbContext context, WKTReader reader) : IAreaAnalysisService
{
  public async Task<AreaAnalysisResponseDto> PerformAnalysisAsync(AreaAnalysisRequestDto areaAnalysisRequestDto, string userId)
  {
    if (string.IsNullOrWhiteSpace(areaAnalysisRequestDto.PolygonA) || string.IsNullOrWhiteSpace(areaAnalysisRequestDto.PolygonB) || string.IsNullOrWhiteSpace(areaAnalysisRequestDto.PolygonC))
    {
      throw new BadRequestException("Please complete geometries A, B, and C.");
    }

    var polygonA = reader.Read(areaAnalysisRequestDto.PolygonA);
    var polygonB = reader.Read(areaAnalysisRequestDto.PolygonB);
    var polygonC = reader.Read(areaAnalysisRequestDto.PolygonC);

    NetTopologySuite.Geometries.Geometry resultGeometry;
    var requiresSaving = false;

    switch (areaAnalysisRequestDto.OperationType)
    {
      case "IntersectAB":
        resultGeometry = polygonA.Intersection(polygonB);
        break;
      case "DifferenceBA":
        resultGeometry = polygonB.Difference(polygonA);
        break;
      case "UnionAB":
        resultGeometry = polygonA.Union(polygonB);
        requiresSaving = true;
        break;
      case "UnionABC":
        resultGeometry = polygonA.Union(polygonB).Union(polygonC);
        requiresSaving = true;
        break;
      default:
        throw new BadRequestException("Operation type is required.");
    }

    if (resultGeometry.IsEmpty)
    {
      return new AreaAnalysisResponseDto
      {
        ResultGeometry = null,
        SurfaceArea = 0,
        Message = "No intersection found."
      };
    }

    if (requiresSaving)
    {
      try
      {
        var analysisResult = new AnalysisResult
        {
          UserId = userId,
          Geometry = resultGeometry,
          SurfaceArea = resultGeometry.Area
        };
        await context.AnalysisResults.AddAsync(analysisResult);
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("Saving analysis result failed due to a database error.");
      }
    }

    return new AreaAnalysisResponseDto
    {
      ResultGeometry = resultGeometry.ToText(),
      SurfaceArea = resultGeometry.Area,
      Message = requiresSaving 
        ? "Analysis completed and geometry saved to database successfully." 
        : "Analysis completed successfully."
    };
  }
}
