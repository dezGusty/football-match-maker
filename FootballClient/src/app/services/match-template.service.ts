import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { firstValueFrom } from 'rxjs';
import { MatchTemplate, CreateMatchTemplateRequest, UpdateMatchTemplateRequest } from '../models/match-template.interface';

@Injectable({
  providedIn: 'root'
})
export class MatchTemplateService {
  private apiUrl = `${environment.apiUrl}/MatchTemplates`;

  constructor(private http: HttpClient) { }

  async getTemplates(): Promise<MatchTemplate[]> {
    return firstValueFrom(this.http.get<MatchTemplate[]>(this.apiUrl));
  }

  async getTemplateById(id: number): Promise<MatchTemplate> {
    return firstValueFrom(this.http.get<MatchTemplate>(`${this.apiUrl}/${id}`));
  }

  async createTemplate(template: CreateMatchTemplateRequest): Promise<MatchTemplate> {
    return firstValueFrom(this.http.post<MatchTemplate>(this.apiUrl, template));
  }

  async updateTemplate(id: number, template: UpdateMatchTemplateRequest): Promise<MatchTemplate> {
    return firstValueFrom(this.http.put<MatchTemplate>(`${this.apiUrl}/${id}`, template));
  }

  async deleteTemplate(id: number): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.apiUrl}/${id}`));
  }
}
