import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { LoginRequest } from '../models/login-request';
import { RegisterRequest } from '../models/register-request';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  baseUrl = 'http://localhost:5044/api/Auth/';

  login(loginCreds: LoginRequest) {
    return this.http.post(this.baseUrl + 'login', loginCreds);
  }

  register(registerCreds: RegisterRequest) {
    return this.http.post(this.baseUrl + 'register', registerCreds);
  }
}
