import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ImportRatingsResult {
  usersImported: number;
  existingUsersUpdated: number;
  ratingHistoryEntriesImported: number;
  errors: string[];
  warnings: string[];
}

export interface PreviewRatingData {
  firebaseDocumentId: string;
  firebasePlayerId: string;
  username: string;
  firstName: string;
  lastName: string;
  originalName: string;
  ratingData: any;
  stars: any;
  keywords: any;
  affinity: any;
  isArchived: boolean;
  hasRatingsArray: boolean;
  hasMostRecentMatches: boolean;
  wouldCreateUser: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class ImportService {
  private apiUrl = environment.apiUrl.replace('/api', ''); // Remove /api suffix since we add it in endpoints

  constructor(private http: HttpClient) {}

  /**
   * Test Firebase connection
   */
  testFirebaseConnection(): Observable<any> {
    return this.http.get(`${this.apiUrl}/api/import/test-connection`);
  }

  /**
   * Preview ratings import without date filtering
   */
  previewRatingsImport(
    collectionName: string = 'ratings'
  ): Observable<{ previewData: PreviewRatingData[] }> {
    const params = new HttpParams().set('collectionName', collectionName);
    return this.http.get<{ previewData: PreviewRatingData[] }>(
      `${this.apiUrl}/api/import/preview/ratings`,
      { params }
    );
  }

  /**
   * Preview ratings import with date filtering
   */
  previewRatingsImportFiltered(
    collectionName: string = 'ratings',
    fromDate?: string,
    toDate?: string
  ): Observable<{
    previewData: PreviewRatingData[];
    fromDate?: string;
    toDate?: string;
  }> {
    let params = new HttpParams().set('collectionName', collectionName);

    if (fromDate) {
      params = params.set('fromDate', fromDate);
    }
    if (toDate) {
      params = params.set('toDate', toDate);
    }

    return this.http.get<{
      previewData: PreviewRatingData[];
      fromDate?: string;
      toDate?: string;
    }>(`${this.apiUrl}/api/import/preview/ratings/filtered`, { params });
  }

  /**
   * Import ratings without date filtering
   */
  importRatings(
    collectionName: string = 'ratings'
  ): Observable<{ result: ImportRatingsResult }> {
    const params = new HttpParams().set('collectionName', collectionName);
    return this.http.post<{ result: ImportRatingsResult }>(
      `${this.apiUrl}/api/import/ratings`,
      {},
      { params }
    );
  }

  /**
   * Import ratings with date filtering
   */
  importRatingsFiltered(
    collectionName: string = 'ratings',
    fromDate?: string,
    toDate?: string
  ): Observable<{
    result: ImportRatingsResult;
    fromDate?: string;
    toDate?: string;
  }> {
    let params = new HttpParams().set('collectionName', collectionName);

    if (fromDate) {
      params = params.set('fromDate', fromDate);
    }
    if (toDate) {
      params = params.set('toDate', toDate);
    }

    return this.http.post<{
      result: ImportRatingsResult;
      fromDate?: string;
      toDate?: string;
    }>(`${this.apiUrl}/api/import/ratings/filtered`, {}, { params });
  }

  /**
   * Get raw collection data for inspection
   */
  getRawCollectionData(collectionName: string): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/api/import/raw/${collectionName}`
    );
  }

  /**
   * Populate test data in Firebase
   */
  populateTestData(
    collectionName: string = 'ratings',
    count: number = 100
  ): Observable<any> {
    const params = new HttpParams()
      .set('collectionName', collectionName)
      .set('count', count.toString());
    return this.http.post(
      `${this.apiUrl}/api/import/populate-test-data`,
      {},
      { params }
    );
  }
}
