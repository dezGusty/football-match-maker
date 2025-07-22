import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  username: string = '';
  password: string = '';
  confirmPassword: string = '';
  errorMessage: string = '';
  successMessage: string = '';

  onRegister() {
    if (!this.username || !this.password || !this.confirmPassword) {
      this.errorMessage = 'Complete all fields!';
      this.successMessage = '';
      return;
    }
    if (this.password !== this.confirmPassword) {
      this.errorMessage = 'The passwords are not the same!';
      this.successMessage = '';
      return;
    }
    this.successMessage = 'Succes!';
    this.errorMessage = '';
    this.username = '';
    this.password = '';
    this.confirmPassword = '';
  }
}
