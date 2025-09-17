import { InjectionToken } from '@angular/core';
import { AppConfig } from './appConfig.models';

export const APP_CONFIG = new InjectionToken<AppConfig>('APP_CONFIG', {
  providedIn: 'root',
  factory: () => ({
    baseUrl: 'http://localhost:9010', //ocelot
    basketUsername: 'basket_username',
    tokenLocalStorage: 'token',
    apiRoot: 'http://localhost:9000', //Catalog
    clientRoot: 'http://localhost:4200', //Angular
    idpAuthority: 'https://localhost:9009', //Identity
    clientId: 'angular-client'
  })
});
