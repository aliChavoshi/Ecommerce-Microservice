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
  setToBasket(basket: IBasket) {
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
    return this.setToBasket(basket);
  }
  removeItemFromBasket(productId: string) {
    const basket = this.getCurrentBasket();
    if (basket?.items.some((x) => x.productId === productId)) {
      basket.items = basket.items.filter((x) => x.productId !== productId);
      if (basket.items.length > 0) {
        //we have another items in the basket
        this.setToBasket(basket);
      } else {
        //basket is EMPTY
        this.deleteBasket(basket.userName);
      }
    }
  }
  increaseItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasket();
    if (!basket) return;
    const index = basket?.items.findIndex((x) => x.productId === item.productId);
    //increase Item
    if (index > 0) {
      basket.items[index].quantity++;
      this.setToBasket(basket);
    } else {
      //add new Item to the basket
      const product = this.mapItemBasketToProduct(item);
      this.addItemToBasket(product, 1);
    }
  }
  decreaseItemQuantity(item: IBasketItem) {
    const basket = this.getCurrentBasket();
    if (!basket) return;
    const index = basket.items.findIndex((x) => x.productId === item.productId);
    if (index > 0) {
      if (basket.items[index]?.quantity >= 2) {
        //decrease
        basket.items[index].quantity--;
        this.setToBasket(basket);
      } else {
        //removeItemFromBasket
        this.removeItemFromBasket(basket.items[index].productId);
      }
    }
  }
  deleteBasket(userName: string) {
    this.http.delete<boolean>(`${this.baseUrl}/basket/DeleteBasketByUserName/${userName}`).pipe(
      tap((response) => {
        if (response) {
          this.basketSource.next(null);
          this.basketTotalSource.next(null);
          localStorage.removeItem(this.cfg.basketUsername);
        }
      })
    );
  }
  //#region PrivateMethods
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
  private mapItemBasketToProduct(item: IBasketItem): IProduct {
    return {
      brands: {
        id: '',
        name: ''
      },
      description: '',
      id: item.productId,
      imageFile: item.imageFile,
      name: item.productName,
      price: item.price,
      summary: '',
      types: {
        id: '',
        name: ''
      }
    };
  }
  private createBasket(): Basket {
    let loginUser = 'Ali Chavoshi';
    const basket = new Basket(loginUser);
    localStorage.setItem(this.cfg.basketUsername, loginUser);
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
  //#endregion
}
