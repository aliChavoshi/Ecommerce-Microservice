import { Component, Input, OnInit } from '@angular/core';
import { IProduct } from '../../shared/models/product';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { BasketService } from '../../basket/basket.service';

@Component({
  selector: 'app-product-item',
  imports: [CurrencyPipe, CommonModule, RouterModule],
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.css', '../store.component.css']
})
export class ProductItemComponent implements OnInit {
  showingBtnCart = false;
  @Input({ required: true }) product!: IProduct;
  constructor(public basketService: BasketService) {}

  addItemToBasket() {
    this.basketService.addItemToBasket(this.product, 1).subscribe((res) => {
      if (res) {
        this.showingBtnCart = false;
      }
    });
  }
  ngOnInit(): void {
    this.calculatingShowAddToCartBtn();
  }
  calculatingShowAddToCartBtn() {
    const currentBasket = this.basketService.getCurrentBasket();
    const result = currentBasket?.items?.find((x) => x.productId == this.product.id);
    if (!result) {
      this.showingBtnCart = true;
    }
  }
}
