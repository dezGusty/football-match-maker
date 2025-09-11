import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ImpersonationService {
  private apiUrl = `${environment.apiUrl}/Impersonation`;
  private isImpersonatingSubject = new BehaviorSubject<boolean>(this.getIsImpersonating());
  public isImpersonating$ = this.isImpersonatingSubject.asObservable();
  
  private impersonatedUserSubject = new BehaviorSubject<any>(this.getImpersonatedUser());
  public impersonatedUser$ = this.impersonatedUserSubject.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private router: Router
  ) { }

  startImpersonation(userId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/start/${userId}`, {}).pipe(
      tap(response => {
        if (response && response.token) {
          // Store the new token
          localStorage.setItem('token', response.token);
          
          // Store impersonation state
          localStorage.setItem('isImpersonating', 'true');
          localStorage.setItem('originalAdminId', response.originalAdminId);
          localStorage.setItem('impersonatedUser', JSON.stringify(response.user));
          
          // Update the behavior subjects
          this.isImpersonatingSubject.next(true);
          this.impersonatedUserSubject.next(response.user);
          
          // Update the auth service's user data
          this.authService.setUserData(response.user);
        }
      })
    );
  }

  stopImpersonation(): Observable<any> {
    // Get the original admin ID from local storage
    const originalAdminId = this.getOriginalAdminId();
    const requestBody = { originalAdminId };
    
    return this.http.post<any>(`${this.apiUrl}/stop`, requestBody).pipe(
      tap(response => {
        if (response && response.token) {
          // Store the new token (admin's token)
          localStorage.setItem('token', response.token);
          
          // Clear impersonation state
          localStorage.removeItem('isImpersonating');
          localStorage.removeItem('originalAdminId');
          localStorage.removeItem('impersonatedUser');
          
          // Update the behavior subjects
          this.isImpersonatingSubject.next(false);
          this.impersonatedUserSubject.next(null);
          
          // Update the auth service's user data
          this.authService.setUserData(response.user);
          
          // Navigate back to the user impersonation page
          this.router.navigate(['/user-impersonation']);
        }
      })
    );
  }

  getIsImpersonating(): boolean {
    return localStorage.getItem('isImpersonating') === 'true';
  }

  getImpersonatedUser(): any {
    const user = localStorage.getItem('impersonatedUser');
    return user ? JSON.parse(user) : null;
  }

  getOriginalAdminId(): string | null {
    return localStorage.getItem('originalAdminId');
  }
  
  // Method to clear impersonation state and update subjects
  clearImpersonationState(): void {
    localStorage.removeItem('isImpersonating');
    localStorage.removeItem('originalAdminId');
    localStorage.removeItem('impersonatedUser');
    
    // Update the behavior subjects
    this.isImpersonatingSubject.next(false);
    this.impersonatedUserSubject.next(null);
  }
}
