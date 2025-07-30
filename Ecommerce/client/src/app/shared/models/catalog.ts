export interface Catalog {
  id: string;
  name: string;
  summary: string;
  description: string;
  imageFile: string;
  brands: Brand;
  types: Type;
  price: number;
}
export class CatalogParams {
  pageIndex = 1;
  pageSize = 9;
  brandId?: string;
  typeId?: string;
  sort!: 'priceAsc' | 'priceDesc';
  search?: string;
}
export interface Brand {
  id: string;
  name: string;
}

export interface Type {
  id: string;
  name: string;
}
