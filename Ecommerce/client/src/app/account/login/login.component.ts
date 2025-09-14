import { Component, inject } from '@angular/core';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private accountService = inject(AccountService);
  handleLogin() {
    this.accountService.login();
  }
}
