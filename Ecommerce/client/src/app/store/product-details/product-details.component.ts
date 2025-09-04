import { Component } from '@angular/core';
import { StoreService } from '../store.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { IProduct } from '../../shared/models/product';
import { DecimalPipe } from '@angular/common';
import { BreadcrumbService } from 'xng-breadcrumb';
import { BasketService } from '../../basket/basket.service';

@Component({
  selector: 'app-product-details',
  imports: [RouterLink, DecimalPipe],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent {
  ExistedInBasket = false;
  private productId!: string;
  product!: IProduct;
  constructor(private storeService: StoreService, private route: ActivatedRoute,
     private bcService: BreadcrumbService, public basketService: BasketService) {
    this.productId = this.route.snapshot.params['id'];
  }
  ngOnInit(): void {
    this.getProductById();
    this.existedInBasket();
  }
  addItemToBasket(){
    this.basketService.addItemToBasket(this.product).subscribe(res=>{
      if(res){
        this.ExistedInBasket = true;
      }
    });
  }
  private existedInBasket() {
    const basket = this.basketService.getCurrentBasket();
    if (basket?.items.some(x => x.productId == this.productId)) {
      this.ExistedInBasket = true;
    }
  }
  private getProductById() {
    this.storeService.getProductById(this.productId).subscribe((res) => {
      this.product = res;
      this.bcService.set('@productDetail', this.product.name);
    });
  }
}
