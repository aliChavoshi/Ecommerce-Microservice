import { inject, Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
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
  private currentUserSource = new ReplaySubject<User | null>(1);
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
    this.setUser(null);
    await this.manager.signoutRedirect();
  }
  logout() {
    localStorage.removeItem(this.config.tokenLocalStorage);
    localStorage.removeItem(this.config.basketUsername);
    this.setUser(null);
    this.router.navigateByUrl('/');
  }
  public get authorizationHeaderValue(): string {
    // console.log('🚀 ~ AccountService ~ token :', this.token);
    // console.log('🚀 ~ AccountService ~ access_token:', this.access_token);
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
  public loadUserFromStorage() {
    const userJson = localStorage.getItem(this.config.tokenLocalStorage);
    if (userJson) {
      const user: User = JSON.parse(userJson);
      this.user = user;
      this.access_token = user.access_token;
      this.token = user.token_type;
      this.currentUserSource.next(user);
    } else {
      this.currentUserSource.next(null);
    }
  }
  private getUser() {
    const userStr = localStorage.getItem(this.config.tokenLocalStorage);
    if (userStr) {
      const userObj = JSON.parse(userStr) as User;
      this.setUser(userObj);
    } else {
      // fallback: از UserManager بگیرید
      this.manager.getUser().then((user) => {
        this.setUser(user);
      });
    }
  }
  private setUser(user: User | null) {
    this.user = user;
    if (user) {
      localStorage.setItem(this.config.tokenLocalStorage, JSON.stringify(user));
      this.token = user.token_type;
      this.access_token = user.access_token;
    } else {
      localStorage.removeItem(this.config.tokenLocalStorage);
      this.token = '';
      this.access_token = '';
    }
    this.currentUserSource.next(user ?? null);
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
