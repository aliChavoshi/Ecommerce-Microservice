import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { BasketService } from '../../basket/basket.service';
import { AsyncPipe } from '@angular/common';
import { IBasketItem } from '../../shared/models/basket';
import { AccountService } from '../../account/account.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterModule, AsyncPipe],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  public accountService = inject(AccountService);
  constructor(public basketService: BasketService) {}

  getBasketCount(items: IBasketItem[]) {
    return items.reduce((sum, item) => sum + item.quantity, 0);
  }

  logout() {
    this.accountService.signout().then((res) => {
      // console.log('🚀 ~ NavbarComponent ~ logout ~ res:', res);
    });
  }
  login() {
    this.accountService.login().then((res) => {
      // console.log('🚀 ~ NavbarComponent ~ login ~ res:', res);
    });
  }
}
