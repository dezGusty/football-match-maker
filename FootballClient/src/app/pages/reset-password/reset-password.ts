import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';

interface SetPasswordRequest {
  token: string;
  password: string;
}

interface ApiResponse {
  message: string;
}

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterModule],
  templateUrl: './reset-password.html',
  styleUrls: ['./reset-password.css'],
})
export class SetPasswordComponent implements OnInit {
  setPasswordForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  token = '';

  private readonly API_URL = `${environment.apiUrl}/Auth`;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
    private auth: AuthService
  ) {
    this.setPasswordForm = this.createForm();
  }

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'] || '';
      console.log(this.token);
      if (!this.token) {
        this.errorMessage = 'Invalid or missing token in URL.';
      }
    });
  }

  private createForm(): FormGroup {
    return this.fb.group(
      {
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(6),
            Validators.maxLength(100),
          ],
        ],
        confirmPassword: ['', [Validators.required]],
      },
      {
        validators: this.passwordMatchValidator,
      }
    );
  }

  private passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password');
    const confirmPassword = formGroup.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    if (password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else {
      if (confirmPassword.errors?.['passwordMismatch']) {
        delete confirmPassword.errors['passwordMismatch'];
        if (Object.keys(confirmPassword.errors).length === 0) {
          confirmPassword.setErrors(null);
        }
      }
      return null;
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.setPasswordForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

  getPasswordStrength(): string {
    const password = this.setPasswordForm.get('password')?.value || '';
    let score = 0;

    if (password.length >= 6) score++;
    if (password.length >= 10) score++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) score++;
    if (/\d/.test(password)) score++;
    if (/[^A-Za-z0-9]/.test(password)) score++;

    if (score <= 2) return 'weak';
    if (score === 3) return 'fair';
    if (score === 4) return 'good';
    return 'strong';
  }

  getPasswordStrengthPercentage(): number {
    const strength = this.getPasswordStrength();
    const percentages = { weak: 25, fair: 50, good: 75, strong: 100 };
    return percentages[strength as keyof typeof percentages] || 0;
  }

  getPasswordStrengthText(): string {
    const strength = this.getPasswordStrength();
    const texts = {
      weak: 'Weak',
      fair: 'Fair',
      good: 'Good',
      strong: 'Strong',
    };
    return texts[strength as keyof typeof texts] || '';
  }

  async onSubmit(): Promise<void> {
    if (this.setPasswordForm.invalid || !this.token) {
      this.setPasswordForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const requestData: SetPasswordRequest = {
      token: this.token,
      password: this.setPasswordForm.get('password')?.value,
    };

    const headers = {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.auth.getToken()}`,
    };

    try {
      const response = await this.http
        .post<ApiResponse>(`${this.API_URL}/set-password`, requestData, {
          headers,
        })
        .toPromise();

      this.successMessage =
        response?.message || 'Password has been successfully set!';
      this.setPasswordForm.reset();

      await this.auth.logout();
      this.router.navigate(['/login']);
    } catch (error) {
      this.handleError(error);
    } finally {
      this.isLoading = false;
    }
  }

  private handleError(error: any): void {
    if (error instanceof HttpErrorResponse) {
      if (error.error?.message) {
        this.errorMessage = error.error.message;
      } else if (error.status === 400) {
        this.errorMessage =
          'Invalid data. Please check the entered information.';
      } else if (error.status === 500) {
        this.errorMessage = 'Server error. Please try again later.';
      } else {
        this.errorMessage = `Unexpected error. Please try again. Status: ${error.status}, ${error}`;
      }
    } else {
      this.errorMessage =
        'Connection error. Please check your internet connection.';
    }

    console.error('Set password error:', error);
  }
}
