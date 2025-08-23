export interface IProduct {
  id: string;
  name: string;
  summary: string;
  description: string;
  imageFile: string;
  brands: IBrand;
  types: IType;
  price: number;
}
export class ProductParams {
  pageIndex = 1;
  pageSize = 9;
  brandId?: string;
  typeId?: string;
  sort: 'priceAsc' | 'priceDesc' = 'priceAsc';
  search?: string;
  count?: number;
}
export interface IBrand {
  id: string;
  name: string;
}

export interface IType {
  id: string;
  name: string;
}
