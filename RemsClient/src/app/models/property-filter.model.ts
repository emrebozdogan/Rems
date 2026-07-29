export interface PropertyFilter {
  parcelNumber?: string;
  lotNumber?: string;
  address?: string;
  propertyType?: string;
  neighborhoodName?: string;
  districtName?: string;
  cityName?: string;
  ownerId?: string;
  pageNumber: number;
  pageSize: number;
}
