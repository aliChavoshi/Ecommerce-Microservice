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
export interface Brand {
  id: string;
  name: string;
}

export interface Type {
  id: string;
  name: string;
}
