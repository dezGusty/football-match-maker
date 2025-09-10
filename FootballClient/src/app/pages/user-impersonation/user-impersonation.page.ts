import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserImpersonationComponent } from '../../components/user-impersonation/user-impersonation.component';

@Component({
  selector: 'app-user-impersonation-page',
  standalone: true,
  imports: [CommonModule, FormsModule, UserImpersonationComponent],
  template: `
    <div class="container mt-4">
      <app-user-impersonation></app-user-impersonation>
    </div>
  `,
  styles: []
})
export class UserImpersonationPage {}
