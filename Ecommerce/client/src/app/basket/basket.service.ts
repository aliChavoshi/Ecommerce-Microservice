import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { IBasket, Basket, IBasketItem } from '../shared/models/basket';
import { APP_CONFIG } from '../core/configs/appConfig.token';
import { IProduct } from '../shared/models/product';
import { UntypedFormArray } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private cfg = inject(APP_CONFIG);
  private baseUrl = this.cfg.baseUrl;
  private basketSource = new BehaviorSubject<IBasket>(new Basket(''));
  basket$ = this.basketSource.asObservable();

  constructor(private http: HttpClient) {}

  getBasket(userName: string) {
    return this.http.get<IBasket>(`${this.baseUrl}/basket/GetBasketByUserName/${userName}`).pipe(tap((basket) => this.basketSource.next(basket)));
  }

  setBasket(basket: IBasket) {
    return this.http.post<IBasket>(`${this.baseUrl}/basket/CreateBasket`, basket).pipe(tap((basket) => this.basketSource.next(basket)));
  }
  getCurrentBasket() {
    return this.basketSource.value;
  }
  addItemToBasket(product: IProduct, quantity: number = 1) {
    const itemToAdd = this.mapProductToItemBasket(product);
    const basket = this.getCurrentBasket() ?? this.createBasket();
    basket.Items = this.addOrUpdateItemBasket(basket.Items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  private mapProductToItemBasket(product: IProduct): IBasketItem {
    return {
      imageFile: product.imageFile,
      price: product.price,
      productId: product.id,
      productName: product.name,
      quantity: 0
    };
  }

  private createBasket(): Basket {
    let loginUser = 'Ali Chavoshi';
    const basket = new Basket(loginUser); // TODO
    localStorage.setItem('basket_username', loginUser);
    return basket;
  }

  private addOrUpdateItemBasket(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    const itemInBasket = items.find((x) => x.productId == itemToAdd.productId);
    if (itemInBasket) {
      itemInBasket.quantity += quantity;
    } else {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    }
    return items;
  }
}
