import { Component} from '@angular/core';
import { Header } from '../header/header';
import {CommonModule} from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-account',
  imports: [Header, CommonModule, FormsModule],
  templateUrl: './account.html',
  styleUrl: './account.css'
})
export class Account {
  username = 'john.doe'; 
  role = 'User'; 
  newPassword = '';
  confirmPassword = '';
  
  changePassword() {
    //to be implemented
  }
}
