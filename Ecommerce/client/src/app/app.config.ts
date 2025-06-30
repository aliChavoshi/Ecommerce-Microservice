import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    /* `provideHttpClient(withInterceptorsFromDi())` is a configuration setup in Angular for providing
     the HttpClient service with interceptors obtained from the dependency injection container. */
    provideHttpClient(withInterceptorsFromDi()),
  ]
};
