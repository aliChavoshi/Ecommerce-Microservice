import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { APP_CONFIG } from '../core/configs/appConfig.token';
import { AppConfig } from '../core/configs/appConfig.models';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private config = inject(APP_CONFIG);
  private currentUserSource = new ReplaySubject<boolean | null>(1);
  currentUser$ = this.currentUserSource.asObservable();
  //
  private manager = new UserManager(getClientSettings(this.config));
  private user?: User | null;
  //
  token = '';
  access_token = '';

  constructor(private http: HttpClient, private router: Router) {
    this.getUser();
  }

  isAuthenticated() {
    return this.user != null && !this.user.expired;
  }
  async login() {
    return await this.manager.signinRedirect();
  }
  async signout() {
    //TODO why async?
    this.setUser(null);
    await this.manager.signoutRedirect();
  }
  logout() {
    localStorage.removeItem(this.config.tokenLocalStorage);
    this.setUser(null);
    this.router.navigateByUrl('/');
  }
  public get authorizationHeaderValue(): string {
    console.log('🚀 ~ AccountService ~ token :', this.token);
    console.log('🚀 ~ AccountService ~ access_token:', this.access_token);
    return this.access_token ? `Bearer ${this.access_token}` : '';
  }

  /* This `finishLogin` method in the `AccountService` class is an asynchronous arrow function that
handles the completion of the login process. Here's a breakdown of what it does: */
  public finishLogin = async (): Promise<User> => {
    const user = await this.manager.signinRedirectCallback();
    this.setUser(user);
    this.token = user.token_type;
    this.access_token = user.access_token;
    return user;
  };

  public finishLogout = () => {
    this.setUser(null);
    return this.manager.signoutRedirectCallback();
  };

  private getUser() {
    this.manager.getUser().then((user) => {
      this.setUser(user);
    });
  }
  private setUser(user: User | null) {
    this.user = user;
    this.currentUserSource.next(user ? !user.expired : null);
  }
}
export function getClientSettings(config: AppConfig): UserManagerSettings {
  return {
    includeIdTokenInSilentRenew: true,
    automaticSilentRenew: true,
    silent_redirect_uri: `${config.clientRoot}/public/silent-callback.html`,
    authority: config.idpAuthority,
    client_id: config.clientId,
    redirect_uri: `${config.clientRoot}/account/signin-callback`,
    scope: 'openid profile eshoppinggateway catalogapi.read catalogapi.write basketapi',
    response_type: 'code',
    post_logout_redirect_uri: `${config.clientRoot}/account/signout-callback`
  };
}
