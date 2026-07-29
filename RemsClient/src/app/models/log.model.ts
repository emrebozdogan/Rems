export interface Log {
  id: string;
  userId: string;
  userName?: string;
  status: string;
  operationType: string;
  description: string;
  timestamp: string;
  ipAddress: string;
}
