import { Component, OnInit } from '@angular/core';
import { ImpersonationService } from '../../services/impersonation.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-impersonation-status',
  templateUrl: './impersonation-status.component.html',
  styleUrls: ['./impersonation-status.component.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class ImpersonationStatusComponent implements OnInit {
  isImpersonating = false;
  impersonatedUser: any = null;

  constructor(
    private impersonationService: ImpersonationService
  ) { }

  ngOnInit(): void {
    this.impersonationService.isImpersonating$.subscribe(isImpersonating => {
      this.isImpersonating = isImpersonating;
    });

    this.impersonationService.impersonatedUser$.subscribe(user => {
      this.impersonatedUser = user;
    });
  }

  stopImpersonation(): void {
    this.impersonationService.stopImpersonation().subscribe(
      () => {
        // Navigation is handled in the service
      },
      (error) => {
        console.error('Error stopping impersonation:', error);
      }
    );
  }
}
