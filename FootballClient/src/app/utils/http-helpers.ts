import { AuthService } from '../services/auth.service';
import { HttpHeaders } from '@angular/common/http';


export function getAuthHeaders(authService: AuthService): HeadersInit {
  const token = authService.getToken();
  return {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` }),
  };
}
