import { AuthService } from '../services/auth.service';
import { HttpHeaders } from '@angular/common/http';


export function getAuthHeaders(authService: AuthService): HeadersInit {
  const token = authService.getToken();
  return {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` }),
  };
}

export function getAngularAuthHeaders(authService: AuthService): HttpHeaders {
  const token = authService.getToken();
  const headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });
  
  if (token) {
    return headers.set('Authorization', `Bearer ${token}`);
  }
  
  return headers;
}
