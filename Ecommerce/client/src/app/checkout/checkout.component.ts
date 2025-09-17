import { Component, inject } from '@angular/core';
import { BasketService } from '../basket/basket.service';
import { AsyncPipe, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { IBasket } from '../shared/models/basket';

@Component({
  selector: 'app-checkout',
  imports: [AsyncPipe, RouterLink, DecimalPipe],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent {
  public basketService = inject(BasketService);

  order(basket: IBasket) {
    this.basketService.checkoutBasket(basket).subscribe({
      error: (res) => {
        console.log('🚀 ~ CheckoutComponent ~ order ~ res:', res);
      }
    });
  }
}
