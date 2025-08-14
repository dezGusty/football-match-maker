import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface PlayerProfileImageResponse {
  message: string;
  imageUrl: string;
}

@Injectable({
  providedIn: 'root',
})
export class PlayerProfileImageService {
  private apiUrl = 'http://localhost:5145/api/players';

  constructor(private http: HttpClient) {}

  uploadProfileImage(
    playerId: number,
    imageFile: File
  ): Observable<PlayerProfileImageResponse> {
    const formData = new FormData();
    formData.append('imageFile', imageFile);

    return this.http.post<PlayerProfileImageResponse>(
      `${this.apiUrl}/${playerId}/profile-image`,
      formData
    );
  }

  deleteProfileImage(playerId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/${playerId}/profile-image`
    );
  }

  validateImageFile(file: File): { isValid: boolean; error?: string } {
    const maxSize = 5 * 1024 * 1024;
    const allowedTypes = [
      'image/jpeg',
      'image/jpg',
      'image/png',
      'image/gif',
      'image/webp',
    ];

    if (!allowedTypes.includes(file.type)) {
      return {
        isValid: false,
        error: 'Please select a valid image file (JPG, PNG, GIF, or WebP).',
      };
    }

    if (file.size > maxSize) {
      return {
        isValid: false,
        error: 'Image file size must be less than 5MB.',
      };
    }

    return { isValid: true };
  }
}
