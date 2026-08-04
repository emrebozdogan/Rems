import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AreaAnalysisRequest, AreaAnalysisResponse } from '../../models/area-analysis.model';

@Injectable({ providedIn: 'root' })
export class AreaAnalysisService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5044/api';

  performAnalysis(request: AreaAnalysisRequest): Observable<AreaAnalysisResponse> {
    return this.http.post<AreaAnalysisResponse>(`${this.apiUrl}/AreaAnalysis`, request);
  }

  saveGeometries(req: { polygonA: string, polygonB: string, polygonC: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/AreaAnalysis/save-geometries`, req);
  }

  getSavedGeometries(): Observable<{ polygonA: string, polygonB: string, polygonC: string }> {
    return this.http.get<{ polygonA: string, polygonB: string, polygonC: string }>(`${this.apiUrl}/AreaAnalysis/saved-geometries`);
  }
}
