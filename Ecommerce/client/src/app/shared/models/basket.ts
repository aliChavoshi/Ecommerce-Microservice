export interface IBasket {
  userName: string;
  items: IBasketItem[];
  totalPrice: number;
}

export interface IBasketItem {
  quantity: number;
  price: number;
  productId: string;
  imageFile: string;
  productName: string;
}

export class Basket implements IBasket {
  constructor(userName: string) {
    this.userName = userName;
    this.totalPrice = 0;
    this.items = [];
  }
  userName: string;
  totalPrice: number;
  items: IBasketItem[];
}
export interface IBasketTotal {
  totalItems: number;
  tax: number;
  shippingTotal: number;
  discount: number;
  totalToPay : number
}
