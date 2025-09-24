import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { getAngularAuthHeaders } from '../utils/http-helpers';
import {
  RatingHistory,
  RatingTrend,
  GetRatingHistoryFilters,
  RatingStatistics,
} from '../models/rating-history.interface';

@Injectable({
  providedIn: 'root',
})
export class RatingHistoryService {
  private readonly apiUrl = `${environment.apiUrl}/ratinghistory`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  async getUserRatingHistory(
    userId: number,
    filters?: GetRatingHistoryFilters
  ): Promise<RatingHistory[]> {
    let params: any = {};
    if (filters) {
      if (filters.fromDate) params.fromDate = filters.fromDate.toISOString();
      if (filters.toDate) params.toDate = filters.toDate.toISOString();
      if (filters.matchId) params.matchId = filters.matchId;
      if (filters.changeReason) params.changeReason = filters.changeReason;
      if (filters.page) params.page = filters.page;
      if (filters.pageSize) params.pageSize = filters.pageSize;
    }

    return firstValueFrom(
      this.http.get<RatingHistory[]>(`${this.apiUrl}/user/${userId}`, {
        headers: getAngularAuthHeaders(this.authService),
        params: params,
      })
    );
  }

  async getUserRatingTrend(
    userId: number,
    lastNMatches?: number
  ): Promise<RatingTrend> {
    let params: any = {};
    if (lastNMatches) {
      params.lastNMatches = lastNMatches;
    }

    return firstValueFrom(
      this.http.get<RatingTrend>(`${this.apiUrl}/user/${userId}/trend`, {
        headers: getAngularAuthHeaders(this.authService),
        params: params,
      })
    );
  }

  async getUserRatingStatistics(userId: number): Promise<RatingStatistics> {
    return firstValueFrom(
      this.http.get<RatingStatistics>(
        `${this.apiUrl}/user/${userId}/statistics`,
        {
          headers: getAngularAuthHeaders(this.authService),
        }
      )
    );
  }

  async getRecentRatingChanges(
    userId: number,
    count: number = 10
  ): Promise<RatingHistory[]> {
    return firstValueFrom(
      this.http.get<RatingHistory[]>(`${this.apiUrl}/user/${userId}/recent`, {
        headers: getAngularAuthHeaders(this.authService),
        params: { count: count.toString() },
      })
    );
  }

  async getUserRatingAtDate(
    userId: number,
    date: Date
  ): Promise<number | null> {
    const response = await firstValueFrom(
      this.http.get<{ rating: number; date: Date } | null>(
        `${this.apiUrl}/user/${userId}/rating-at-date`,
        {
          headers: getAngularAuthHeaders(this.authService),
          params: { date: date.toISOString() },
        }
      )
    );
    return response?.rating ?? null;
  }

  async getMatchRatingChanges(matchId: number): Promise<RatingHistory[]> {
    return firstValueFrom(
      this.http.get<RatingHistory[]>(`${this.apiUrl}/match/${matchId}`, {
        headers: getAngularAuthHeaders(this.authService),
      })
    );
  }
}
