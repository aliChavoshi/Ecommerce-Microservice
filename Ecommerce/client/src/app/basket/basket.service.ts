import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';
import { IBasket, Basket } from '../shared/models/basket';
import { APP_CONFIG } from '../core/configs/appConfig.token';

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
    return this.http.get<IBasket>(`${this.baseUrl}/basket/GetBasketByUserName/${userName}`)
    .pipe(tap((basket) => this.basketSource.next(basket)));
  }

  setBasket(basket: IBasket) {
    return this.http.post<IBasket>(`${this.baseUrl}/basket/CreateBasket`, basket)
      .pipe(tap((basket) => this.basketSource.next(basket)));
  }
  getCurrentBasket(){
    return this.basketSource.value;
  }
}
