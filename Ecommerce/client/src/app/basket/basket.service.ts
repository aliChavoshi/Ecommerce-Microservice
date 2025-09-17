import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, EMPTY, map, Observable, of, switchMap, tap } from 'rxjs';
import { IBasket, Basket, IBasketItem, IBasketTotal } from '../shared/models/basket';
import { APP_CONFIG } from '../core/configs/appConfig.token';
import { IProduct } from '../shared/models/product';
import { AccountService } from '../account/account.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  private accountService = inject(AccountService);
  private router = inject(Router);
  private cfg = inject(APP_CONFIG);
  private baseUrl = this.cfg.baseUrl;
  private basketSource = new BehaviorSubject<IBasket | null>(null);
  basket$ = this.basketSource.asObservable();
  //
  private basketTotalSource = new BehaviorSubject<IBasketTotal | null>(null);
  basketTotal$ = this.basketTotalSource.asObservable();
  //

  constructor(private http: HttpClient) {}

  getBasket(userName: string): Observable<IBasket> {
    return this.http.get<IBasket>(`${this.baseUrl}/basket/GetBasketByUserName/${userName}`).pipe(
      tap((basket) => {
        this.basketSource.next(basket), this.calculateBasketTotal();
      })
    );
  }
  setToBasket(basket: IBasket): Observable<IBasket> {
    return this.http.post<IBasket>(`${this.baseUrl}/basket/CreateBasket`, basket).pipe(
      tap((basket) => {
        this.basketSource.next(basket), this.calculateBasketTotal();
      })
    );
  }
  getCurrentBasket(): IBasket | null {
    return this.basketSource.getValue();
  }
  addItemToBasket(product: IProduct, quantity: number = 1): Observable<IBasket> {
    const itemToAdd = this.mapProductToItemBasket(product);

    return this.getCurrentBasketAsync().pipe(
      switchMap((basket) => {
        basket.items = this.addOrUpdateItemBasket(basket.items, itemToAdd, quantity);
        return this.setToBasket(basket);
      })
    );
  }
  removeItemFromBasket(productId: string): Observable<IBasket | boolean> {
    const basket = this.getCurrentBasket();
    if (basket?.items.some((x) => x.productId === productId)) {
      basket.items = basket.items.filter((x) => x.productId !== productId);
      if (basket.items.length > 0) {
        //we have another items in the basket
        return this.setToBasket(basket);
      } else {
        //basket is EMPTY
        return this.deleteBasket(basket.userName);
      }
    }
    return EMPTY;
  }
  increaseItemQuantity(item: IBasketItem): Observable<IBasket> {
    const basket = this.getCurrentBasket();
    if (!basket) return EMPTY;
    const index = basket?.items.findIndex((x) => x.productId === item.productId);
    //increase Item
    if (index > 0) {
      basket.items[index].quantity++;
      return this.setToBasket(basket);
    } else {
      //add new Item to the basket
      const product = this.mapItemBasketToProduct(item);
      return this.addItemToBasket(product, 1);
    }
  }
  decreaseItemQuantity(item: IBasketItem): Observable<IBasket | boolean> {
    const basket = this.getCurrentBasket();
    if (!basket) return EMPTY;
    const index = basket.items.findIndex((x) => x.productId == item?.productId);
    if (index >= 0) {
      if (basket.items[index]?.quantity >= 2) {
        //decrease
        basket.items[index].quantity--;
        return this.setToBasket(basket);
      } else {
        //removeItemFromBasket
        return this.removeItemFromBasket(basket?.items[index]?.productId);
      }
    }
    return EMPTY;
  }
  deleteBasket(userName: string) {
    return this.http.delete<boolean>(`${this.baseUrl}/basket/DeleteBasketByUserName/${userName}`).pipe(
      tap((response) => {
        if (response) {
          this.basketSource.next(null);
          this.basketTotalSource.next(null);
          localStorage.removeItem(this.cfg.basketUsername);
        }
      })
    );
  }
  checkoutBasket(body: IBasket) {
    console.log("🚀 ~ BasketService ~ checkoutBasket ~ body:", body)
    // const httpOptions = {
    //   headers: new HttpHeaders({
    //     'Content-Type': 'application/json',
    //     Authorization: this.accountService.authorizationHeaderValue
    //   })
    // };
    return this.http.post<IBasket>(this.baseUrl + '/Basket/CheckoutV2', body).pipe(
      map((_) => {
        this.basketSource.next(null);
        this.router.navigateByUrl('/');
        this.deleteBasket(body.userName);
      })
    );
  }
  getSubItems() {
    const basket = this.getCurrentBasket();
    return basket?.items.reduce((pre, cur) => {
      return pre + cur.quantity;
    }, 0);
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
  public mapProductToItemBasket(product: IProduct): IBasketItem {
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
  private createBasket(): Observable<Basket> {
    return this.accountService.currentUser$.pipe(
      map((res) => {
        if (res && res.profile?.given_name) {
          const userName = res.profile.given_name;
          localStorage.setItem(this.cfg.basketUsername, userName);
          return new Basket(userName);
        }
        return new Basket(''); // fallback
      })
    );
  }
  // یک تابع async برای گرفتن basket
  private getCurrentBasketAsync(): Observable<Basket> {
    const basket = this.getCurrentBasket();
    if (basket) return of(basket); // موجود بود، sync
    return this.createBasket(); // اگر نبود، async بساز
  }
  private addOrUpdateItemBasket(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
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
