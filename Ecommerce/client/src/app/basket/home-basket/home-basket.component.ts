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
}
