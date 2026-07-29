export interface PaginatedResults<T> {
  data: T[];
  totalCount: number;
  pageSize: number;
  pageNumber: number;
  totalPages: number;
}
