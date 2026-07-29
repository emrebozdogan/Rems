import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../../models/user.model';
import { UserFilter } from '../../models/user-filter.model';
import { PaginatedResults } from '../../models/paginated-results.model';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5044/api';

  getUsers(filter: UserFilter): Observable<PaginatedResults<User>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.id) params = params.set('id', filter.id);
    if (filter.name) params = params.set('name', filter.name);
    if (filter.email) params = params.set('email', filter.email);
    if (filter.role) params = params.set('role', filter.role);

    return this.http.get<PaginatedResults<User>>(`${this.apiUrl}/User`, { params });
  }

  createUser(userData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/User/create`, userData);
  }

  updateUser(userId: string, userData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/User/update/${userId}`, userData);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/User/delete/${userId}`);
  }
}
