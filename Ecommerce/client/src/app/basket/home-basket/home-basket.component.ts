import { Component } from '@angular/core';
import { BasketService } from '../basket.service';
import { AsyncPipe, DecimalPipe } from '@angular/common';
import { IBasketItem } from '../../shared/models/basket';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home-basket',
  imports: [AsyncPipe, DecimalPipe, RouterLink],
  templateUrl: './home-basket.component.html',
  styleUrl: './home-basket.component.css'
})
export class HomeBasketComponent {
  constructor(public basketService: BasketService) {}

  deleteBasket() {
    const basket = this.basketService.getCurrentBasket();
    if (basket)
      this.basketService.deleteBasket(basket.userName).subscribe((res) => {
        // console.log('🚀 ~ HomeBasketComponent ~ deleteBasket ~ res:', res);
      });
  }
  increaseItemQuantity(item: IBasketItem) {
    this.basketService.increaseItemQuantity(item).subscribe((res) => {
      // console.log('🚀 ~ HomeBasketComponent ~ increaseItemQuantity ~ res:', res);
    });
  }
  decreaseItemQuantity(item: IBasketItem) {
    this.basketService.decreaseItemQuantity(item).subscribe((res) => {});
  }
  deleteItemFromBasket(productId: string) {
    this.basketService.removeItemFromBasket(productId).subscribe((res) => {
      console.log('🚀 ~ HomeBasketComponent ~ deleteItemFromBasket ~ res:', res);
    });
  }
}
