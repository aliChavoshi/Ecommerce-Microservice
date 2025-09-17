import { Component, CUSTOM_ELEMENTS_SCHEMA, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './core/navbar/navbar.component';
import { FooterComponent } from './core/footer/footer.component';
import { HeaderComponent } from './core/header/header.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BasketService } from './basket/basket.service';
import { APP_CONFIG } from './core/configs/appConfig.token';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, FooterComponent, HeaderComponent, NgxSpinnerModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppComponent implements OnInit {
  private config = inject(APP_CONFIG);
  private accountService = inject(AccountService);
  constructor(private basketService: BasketService) {}

  ngOnInit(): void {
    this.accountService.loadUserFromStorage(); // بارگذاری user

    this.accountService.currentUser$.subscribe((user) => {
      if (user) {
        const loggedUser = localStorage.getItem(this.config.basketUsername);
        if (loggedUser) {
          this.basketService.getBasket(loggedUser).subscribe();
        }
      }
    });
  }
}
