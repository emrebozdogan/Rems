export interface Property {
  id: string;
  userId?: string;
  parcelNumber: string;
  lotNumber: string;
  address: string;
  geometry: string;
  propertyType: string;
  imagePath?: string;
  neighborhoodName: string;
  districtName: string;
  cityName: string;
  neighborhoodId?: string;
  districtId?: string;
  cityId?: string;
}
