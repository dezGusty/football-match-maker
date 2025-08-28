import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface SetPasswordRequest {
  token: string;
  password: string;
}

export interface SetPasswordResponse {
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class PasswordService {
  private readonly apiUrl = `${environment.apiUrl}/Auth`;
  constructor(private http: HttpClient) {}

  setPassword(request: SetPasswordRequest): Observable<SetPasswordResponse> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
    });

    return this.http.post<SetPasswordResponse>(
      `${this.apiUrl}/set-password`,
      request,
      { headers }
    );
  }

  validatePasswordStrength(password: string): {
    score: number;
    strength: 'weak' | 'fair' | 'good' | 'strong';
    suggestions: string[];
  } {
    let score = 0;
    const suggestions: string[] = [];

    if (password.length >= 6) {
      score++;
    } else {
      suggestions.push('Folosește cel puțin 6 caractere');
    }

    if (password.length >= 10) {
      score++;
    } else if (password.length >= 6) {
      suggestions.push('Consideră folosirea a cel puțin 10 caractere');
    }

    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) {
      score++;
    } else {
      suggestions.push('Combină litere mari și mici');
    }

    if (/\d/.test(password)) {
      score++;
    } else {
      suggestions.push('Adaugă cel puțin o cifră');
    }

    if (/[^A-Za-z0-9]/.test(password)) {
      score++;
    } else {
      suggestions.push('Adaugă caractere speciale (!@#$%^&*)');
    }

    let strength: 'weak' | 'fair' | 'good' | 'strong';
    if (score <= 2) strength = 'weak';
    else if (score === 3) strength = 'fair';
    else if (score === 4) strength = 'good';
    else strength = 'strong';

    return { score, strength, suggestions };
  }
}
