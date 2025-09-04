import { Component } from '@angular/core';
import { BasketService } from '../basket.service';
import { AsyncPipe, DecimalPipe } from '@angular/common';
import { IProduct } from '../../shared/models/product';

@Component({
  selector: 'app-home-basket',
  imports: [AsyncPipe, DecimalPipe],
  templateUrl: './home-basket.component.html',
  styleUrl: './home-basket.component.css'
})
export class HomeBasketComponent {
  constructor(public basketService: BasketService) {}

  deleteBasket() {
    const basket = this.basketService.getCurrentBasket();
    if (basket) this.basketService.deleteBasket(basket.userName).subscribe((res) => {
      console.log("🚀 ~ HomeBasketComponent ~ deleteBasket ~ res:", res)
    });
  }
}
