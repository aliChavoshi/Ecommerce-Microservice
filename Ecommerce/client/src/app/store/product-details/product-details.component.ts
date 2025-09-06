import { Component } from '@angular/core';
import { StoreService } from '../store.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IProduct } from '../../shared/models/product';
import { DecimalPipe } from '@angular/common';
import { BreadcrumbService } from 'xng-breadcrumb';
import { BasketService } from '../../basket/basket.service';
import { IBasketItem } from '../../shared/models/basket';

@Component({
  selector: 'app-product-details',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent {
  productId!: string;
  product!: IProduct;
  itemBasket: IBasketItem | undefined;
  ExistedInBasket = false;
  //
  constructor(private storeService: StoreService, private route: ActivatedRoute, private bcService: BreadcrumbService, public basketService: BasketService) {
    this.productId = this.route.snapshot.params['id'];
  }
  ngOnInit(): void {
    this.getProductById();
    this.getItemBasket();
  }
  addItemToBasket() {
    this.basketService.addItemToBasket(this.product).subscribe((res) => {
      if (res) {
        this.ExistedInBasket = true;
        if (!this.itemBasket) {
          this.itemBasket = res.items.find((x) => x.productId == this.productId)!;
        }
      }
    });
  }
  increaseItemQuantity() {
    this.basketService.increaseItemQuantity(this.itemBasket!).subscribe((res) => {
      if (res) {
        this.getItemBasket();
      }
    });
  }
  decreaseItemQuantity() {
    if(this.itemBasket){
      this.basketService.decreaseItemQuantity(this.itemBasket).subscribe((res) => {
        if (res) {
          this.getItemBasket();
        }
      });
    }
  }
  private getItemBasket() {
    const basket = this.basketService.getCurrentBasket();
    if (basket && basket?.items?.some((x) => x.productId == this.productId)) {
      this.ExistedInBasket = true;
      this.itemBasket = basket.items.find((x) => x.productId === this.productId)!;
    }else{
      this.ExistedInBasket = false;
      this.itemBasket = undefined;
    }
  }
  private getProductById() {
    this.storeService.getProductById(this.productId).subscribe((res) => {
      this.product = res;
      this.bcService.set('@productDetail', this.product.name);
    });
  }
}
