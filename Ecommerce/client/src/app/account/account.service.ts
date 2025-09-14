import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { User, UserManager, UserManagerSettings } from 'oidc-client';
import { APP_CONFIG } from '../core/configs/appConfig.token';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private config = inject(APP_CONFIG);
  private currentUserSource = new ReplaySubject<unknown>(1);
  currentUser$ = this.currentUserSource.asObservable();
  //
  private manager = new UserManager(getClientSettings());
  private user?: User | null;
  //
  token = '';
  access_token = '';

  constructor(private http: HttpClient, private router: Router) {
    this.manager.getUser().then((user) => {
      this.user = user;
      this.currentUserSource.next(this.isAuthenticated());
    });
  }

  isAuthenticated() {
    return this.user != null && !this.user.expired;
  }
  login() {
    return this.manager.signinRedirect();
  }
  async signout() {
    await this.manager.signoutRedirect();
  }
  logout() {
    localStorage.removeItem(this.config.tokenLocalStorage);
    this.currentUserSource.next(null);
    this.router.navigateByUrl('/');
  }
  public get authorizationHeaderValue(): string {
    console.log('🚀 ~ AccountService ~ token :', this.token);
    console.log('🚀 ~ AccountService ~ access_token:', this.access_token);

    return `${this.token} ${this.access_token}`;
  }

  public finishLogin = (): Promise<User> => {
    return this.manager.signinRedirectCallback().then((user) => {
      this.currentUserSource.next(this.checkUser(user));
      this.token = user.token_type;
      this.access_token = user.access_token;
      return user;
    });
  };

  public finishLogout = () => {
    this.user = null;
    return this.manager.signoutRedirectCallback();
  };
  private checkUser = (user: User): boolean => {
    console.log('inside check user');
    console.log(user);
    return !!user && !user.expired;
  };
}
export function getClientSettings(): UserManagerSettings {
  const config = inject(APP_CONFIG);
  return {
    includeIdTokenInSilentRenew: true,
    automaticSilentRenew: true,
    silent_redirect_uri: `${config.clientRoot}/assets/silent-callback.html`,
    authority: config.idpAuthority,
    client_id: config.clientId,
    redirect_uri: `${config.clientRoot}/account/signin-callback`,
    scope: 'openid profile eshoppinggateway',
    response_type: 'code',
    post_logout_redirect_uri: `${config.clientRoot}/account/signout-callback`
  };
}
