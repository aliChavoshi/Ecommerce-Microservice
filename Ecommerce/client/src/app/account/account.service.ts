import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private currentUserSource = new BehaviorSubject<unknown>(null);
  currentUser$ = this.currentUserSource.asObservable();
  constructor() {}

  finishLogin(): Promise<any> {
    throw new Error('Method not implemented.');
  }
  login() {
    throw new Error('Method not implemented.');
  }
}
