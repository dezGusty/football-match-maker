import { Component, OnInit, OnDestroy } from '@angular/core';
import { ImpersonationService } from '../../services/impersonation.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-impersonation-status',
  templateUrl: './impersonation-status.component.html',
  styleUrls: ['./impersonation-status.component.scss'],
  standalone: true,
  imports: [CommonModule]
})
export class ImpersonationStatusComponent implements OnInit, OnDestroy {
  isImpersonating = false;
  impersonatedUser: any = null;
  private impersonationEndedHandler: any;

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
    
    // Define the handler and store a reference to it
    this.impersonationEndedHandler = () => {
      this.isImpersonating = false;
      this.impersonatedUser = null;
    };
    
    // Listen for impersonation ended event (triggered by logout)
    window.addEventListener('impersonation-ended', this.impersonationEndedHandler);
  }
  
  ngOnDestroy(): void {
    // Clean up event listener with the same handler reference
    window.removeEventListener('impersonation-ended', this.impersonationEndedHandler);
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
