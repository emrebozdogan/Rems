export interface AreaAnalysisRequest {
  polygonA: string;
  polygonB: string;
  polygonC: string;
  operationType: string;
}

export interface AreaAnalysisResponse {
  resultGeometry: string | null;
  surfaceArea: number;
  message: string;
}
