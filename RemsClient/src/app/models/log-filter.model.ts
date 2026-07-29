export interface LogFilter {
  userName?: string;
  status?: string;
  operationType?: string;
  description?: string;
  timestamp?: string;
  ipAddress?: string;
  pageNumber: number;
  numberOfLogs: number;
}
