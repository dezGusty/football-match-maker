import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { appConfig } from './app/app.config';
import { App } from './app/app';
import { jwtInterceptor } from './app/interceptors/jwt.interceptor';

bootstrapApplication(App, {
  ...appConfig,
  providers: [...(appConfig.providers ?? []), provideHttpClient(withInterceptors([jwtInterceptor]))],
}).catch((err) => console.error(err));
