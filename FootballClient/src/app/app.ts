import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationComponent } from './components/notification/notification.component';
import { ImpersonationStatusComponent } from './components/impersonation-status/impersonation-status.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NotificationComponent, ImpersonationStatusComponent],
  template: ` 
    <router-outlet />
    <app-notification />
    <app-impersonation-status />
  `,
})
export class App {
  protected readonly title = signal('Football Manager');
}
