import { InjectionToken } from '@angular/core';
import { AppConfig } from './appConfig.models';

export const APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG', {
  providedIn: 'root',
  factory: () => ({
    baseUrl: 'http://localhost:9010',
    basketUsername: 'basket_username'
  })
});
