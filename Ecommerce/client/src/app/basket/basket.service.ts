import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { IBasket, Basket, IBasketItem, IBasketTotal } from '../shared/models/basket';
import { APP_CONFIG } from '../core/configs/appConfig.token';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private cfg = inject(APP_CONFIG);
  private baseUrl = this.cfg.baseUrl;
  private basketSource = new BehaviorSubject<IBasket | null>(null);
  basket$ = this.basketSource.asObservable();
  //
  private basketTotalSource = new BehaviorSubject<IBasketTotal | null>(null);
  basketTotal$ = this.basketTotalSource.asObservable();
  //

  constructor(private http: HttpClient) {}

  getBasket(userName: string) {
    return this.http.get<IBasket>(`${this.baseUrl}/basket/GetBasketByUserName/${userName}`).pipe(
      tap((basket) => {
        this.basketSource.next(basket), this.calculateBasketTotal();
      })
    );
  }
  setBasket(basket: IBasket) {
    return this.http.post<IBasket>(`${this.baseUrl}/basket/CreateBasket`, basket).pipe(
      tap((basket) => {
        this.basketSource.next(basket), this.calculateBasketTotal();
      })
    );
  }
  getCurrentBasket() {
    return this.basketSource.getValue();
  }
  addItemToBasket(product: IProduct, quantity: number = 1): Observable<IBasket> {
    const itemToAdd = this.mapProductToItemBasket(product);
    const basket = this.getCurrentBasket() ?? this.createBasket();
    basket.items = this.addOrUpdateItemBasket(basket.items, itemToAdd, quantity);
    return this.setBasket(basket);
  }
  private calculateBasketTotal() {
    const basket = this.getCurrentBasket();
    if (!basket) return;

    const totalItems = basket.items.reduce((prev, item) => prev + item.price * item.quantity, 0);

    const discount = 0; // مثلا ثابت یا بر اساس کوپن
    const shippingTotal = 0; // هزینه ارسال
    const tax = totalItems * 0.09;

    const totalToPay = totalItems + shippingTotal + tax - discount;

    this.basketTotalSource.next({
      totalItems: totalItems,
      discount,
      shippingTotal,
      tax,
      totalToPay
    });
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
    const basket = new Basket(loginUser);
    localStorage.setItem('basket_username', loginUser);
    return basket;
  }

  private addOrUpdateItemBasket(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    console.log('🚀 ~ BasketService ~ addOrUpdateItemBasket ~ itemToAdd:', itemToAdd);
    console.log('🚀 ~ BasketService ~ addOrUpdateItemBasket ~ items:', items);
    const itemInBasket = items?.find((x) => x.productId == itemToAdd.productId);
    if (itemInBasket) {
      itemInBasket.quantity += quantity;
    } else {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    }
    return items;
  }
}
