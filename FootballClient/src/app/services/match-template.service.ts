import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';
import {
  MatchTemplate,
  CreateMatchTemplateRequest,
  UpdateMatchTemplateRequest,
} from '../models/match-template.interface';
import { getAngularAuthHeaders } from '../utils/http-helpers';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class MatchTemplateService {
  private apiUrl = `${environment.apiUrl}/MatchTemplates`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  async getTemplates(): Promise<MatchTemplate[]> {
    return firstValueFrom(
      this.http.get<MatchTemplate[]>(this.apiUrl, {
        headers: getAngularAuthHeaders(this.authService),
      })
    );
  }

  async getTemplateById(id: number): Promise<MatchTemplate> {
    return firstValueFrom(
      this.http.get<MatchTemplate>(`${this.apiUrl}/${id}`, {
        headers: getAngularAuthHeaders(this.authService),
      })
    );
  }

  async createTemplate(
    template: CreateMatchTemplateRequest
  ): Promise<MatchTemplate> {
    return firstValueFrom(
      this.http.post<MatchTemplate>(this.apiUrl, template, {
        headers: getAngularAuthHeaders(this.authService),
      })
    );
  }

  async updateTemplate(
    id: number,
    template: UpdateMatchTemplateRequest
  ): Promise<MatchTemplate> {
    return firstValueFrom(
      this.http.put<MatchTemplate>(`${this.apiUrl}/${id}`, template, {
        headers: getAngularAuthHeaders(this.authService),
      })
    );
  }

  async deleteTemplate(id: number): Promise<void> {
    return firstValueFrom(
      this.http.delete<void>(`${this.apiUrl}/${id}`, {
        headers: getAngularAuthHeaders(this.authService),
      })
    );
  }
}
