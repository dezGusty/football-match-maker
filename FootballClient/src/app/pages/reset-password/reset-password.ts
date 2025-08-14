import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';

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
  imports: [
    ReactiveFormsModule,
    CommonModule // pentru *ngIf, *ngFor etc.
  ],
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

  private readonly API_URL = 'https://localhost:5145/api/Auth'; 

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private http: HttpClient
  ) {
    this.setPasswordForm = this.createForm();
  }

  ngOnInit(): void {
    // Obține token-ul din URL parameters
    this.route.queryParams.subscribe((params) => {
      this.token = params['token'] || '';
      if (!this.token) {
        this.errorMessage = 'Token invalid sau lipsă din URL.';
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
      weak: 'Slabă',
      fair: 'Acceptabilă',
      good: 'Bună',
      strong: 'Puternică',
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

    try {
      const response = await this.http
        .post<ApiResponse>(`${this.API_URL}/set-password`, requestData)
        .toPromise();

      this.successMessage =
        response?.message || 'Parola a fost setată cu succes!';
      this.setPasswordForm.reset();

      // Opțional: redirecționează la login după 3 secunde
      setTimeout(() => {
        this.router.navigate(['/login']);
      }, 3000);
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
        this.errorMessage = 'Date invalide. Verifică informațiile introduse.';
      } else if (error.status === 500) {
        this.errorMessage =
          'Eroare server. Te rugăm să încerci din nou mai târziu.';
      } else {
        this.errorMessage =
          'A apărut o eroare neașteptată. Te rugăm să încerci din nou.';
      }
    } else {
      this.errorMessage =
        'Eroare de conectare. Verifică conexiunea la internet.';
    }

    console.error('Set password error:', error);
  }
}
