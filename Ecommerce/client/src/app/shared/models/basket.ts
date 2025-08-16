export interface IBasket {
  userName: string;
  Items: IBasketItem[];
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
    this.Items = [];
    this.totalPrice = 0;
  }
  userName: string;
  Items: IBasketItem[];
  totalPrice: number;
}
