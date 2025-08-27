import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationComponent } from './components/notification/notification.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NotificationComponent],
  template: ` 
    <router-outlet />
    <app-notification />
  `,
})
export class App {
  protected readonly title = signal('FootballClient');
}
