import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Property } from '../../models/property.model';
import { City } from '../../models/city.model';
import { District } from '../../models/district.model';
import { Neighborhood } from '../../models/neighborhood.model';
import { PaginatedResults } from '../../models/paginated-results.model';
import { PropertyFilter } from '../../models/property-filter.model';

@Injectable({ providedIn: 'root' })
export class PropertiesService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5044/api';

  getProperties(filter: PropertyFilter): Observable<PaginatedResults<Property>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber)
      .set('pageSize', filter.pageSize);

    if (filter.cityName) params = params.set('cityName', filter.cityName);
    if (filter.districtName) params = params.set('districtName', filter.districtName);
    if (filter.neighborhoodName) params = params.set('neighborhoodName', filter.neighborhoodName);
    if (filter.propertyType) params = params.set('propertyType', filter.propertyType);
    if (filter.parcelNumber) params = params.set('parcelNumber', filter.parcelNumber);
    if (filter.lotNumber) params = params.set('lotNumber', filter.lotNumber);
    if (filter.address) params = params.set('address', filter.address);
    if (filter.ownerId) params = params.set('ownerId', filter.ownerId);

    return this.http.get<PaginatedResults<Property>>(`${this.apiUrl}/Property`, { params });
  }

  createProperty(propertyData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/Property`, propertyData);
  }

  uploadImage(propertyId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.patch(`${this.apiUrl}/Property/${propertyId}/image`, formData);
  }

  importPropertiesFromExcel(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post(`${this.apiUrl}/Property/import`, formData);
  }

  getCities(): Observable<City[]> {
    return this.http.get<City[]>(`${this.apiUrl}/Cities`);
  }

  getDistrictsByCityId(cityId: string): Observable<District[]> {
    return this.http.get<District[]>(`${this.apiUrl}/Districts/by-city/${cityId}`);
  }

  getNeighborhoodsByDistrictId(districtId: string): Observable<Neighborhood[]> {
    return this.http.get<Neighborhood[]>(`${this.apiUrl}/Neighborhoods/by-district/${districtId}`);
  }

  getPropertyImage(propertyId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/Property/${propertyId}/image`, { responseType: 'blob' });
  }

  deleteProperty(propertyId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Property/${propertyId}`);
  }

  updateProperty(propertyData: any) {
    return this.http.put(`${this.apiUrl}/Property`, propertyData);
  }
}
