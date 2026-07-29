using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IAreaAnalysisService
{
  Task<AreaAnalysisResponseDto> PerformAnalysisAsync(AreaAnalysisRequestDto areaAnalysisRequestDto, string userId);
}
