import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface VersionResponse {
  version: string;
}

@Injectable({
  providedIn: 'root',
})
export class VersionService {
  private readonly url = `${environment.apiUrl}/version`;

  constructor(private http: HttpClient) {}

  getVersion(): Observable<VersionResponse> {
    return this.http.get<VersionResponse>(this.url);
  }
}