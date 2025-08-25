import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatchService } from '../../services/match.service';
import { CreateMatchRequest } from '../../models/create-match.interface';

@Component({
  selector: 'app-create-match',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-match.component.html',
  styleUrls: ['./create-match.component.css'],
})
export class CreateMatchComponent {
  matchForm = {
    matchDate: '',
    location: '',
    cost: null as number | null,
    teamAName: '',
    teamBName: '',
  };

  loading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private matchService: MatchService,
    private router: Router
  ) {
    // Set minimum date to today
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.matchForm.matchDate = tomorrow.toISOString().slice(0, 16);
  }

  async onSubmit() {
    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    try {
      // Validate form
      if (!this.matchForm.matchDate) {
        throw new Error('Data meciului este obligatorie');
      }

      if (!this.matchForm.location) {
        throw new Error('Locația este obligatorie');
      }

      // Prepare request
      const createMatchRequest: CreateMatchRequest = {
        matchDate: new Date(this.matchForm.matchDate).toISOString(),
        status: 1, // Open status
        location: this.matchForm.location,
        cost: this.matchForm.cost || undefined,
        teamAName: this.matchForm.teamAName || undefined,
        teamBName: this.matchForm.teamBName || undefined,
      };

      const createdMatch = await this.matchService.createNewMatch(
        createMatchRequest
      );

      this.successMessage = 'Meciul a fost creat cu succes!';

      setTimeout(() => {
        this.router.navigate(['/home']);
      }, 2000);
    } catch (error: any) {
      this.errorMessage = error.message || 'Eroare la crearea meciului';
      console.error('Error creating match:', error);
    } finally {
      this.loading = false;
    }
  }

  onCancel() {
    this.router.navigate(['/home']);
  }
}
