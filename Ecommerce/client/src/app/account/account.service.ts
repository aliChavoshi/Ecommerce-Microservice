import { Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { User } from '../shared/models/user';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private currentUserSource = new ReplaySubject<unknown>(1);
  currentUser$ = this.currentUserSource.asObservable();
  private user?: User | null;
  token = '';
  access_token = '';

  constructor(private http: HttpClient, private router: Router) {}

  isAuthenticated() {
    return this.user != null && !this.user.expired;
  }
  login() {
    throw new Error('Method not implemented.');
  }
  async signout() {}

  public get authorizationHeaderValue(): string {
    console.log('🚀 ~ AccountService ~ token :', this.token);
    console.log('🚀 ~ AccountService ~ access_token:', this.access_token);

    return `${this.token} ${this.access_token}`;
  }
  logout() {
    
  }
  finishLogin(): Promise<any> {
    throw new Error('Method not implemented.');
  }
}
