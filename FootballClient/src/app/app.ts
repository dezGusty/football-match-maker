import { Component, signal } from '@angular/core';
import { Home } from './home/home';
import {Header} from './header/header' ;

@Component({
  selector: 'app-root',
  imports: [Home, Header],
  template: `
  <app-header />
  <app-home />
  `
})
export class App {
  protected readonly title = signal('FootballClient');
}
