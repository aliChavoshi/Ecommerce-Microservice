import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IBrand, IProduct, ProductParams, IType } from '../shared/models/product';
import { IPaginate } from '../shared/models/paginate';
import { BehaviorSubject, map, Observable, Subject } from 'rxjs';
import { APP_CONFIG } from '../core/configs/appConfig.token';

@Injectable({
  providedIn: 'root'
})
export class StoreService {
  private cfg = inject(APP_CONFIG);
  private baseUrl = this.cfg.baseUrl;
  private paramsSource = new BehaviorSubject<ProductParams>(new ProductParams());
  params$ = this.paramsSource.asObservable();

  constructor(private http: HttpClient) {}

  getProductById(productId: string) {
    return this.http.get<IProduct>(`${this.baseUrl}/Catalog/GetProductById/${productId}`);
  }

  getCurrentParams() {
    return this.paramsSource.value;
  }

  setParams(params: ProductParams) {
    this.paramsSource.next(params);
  }

  getAllCatalogs() {
    let params = this.generateCatalogParams(this.getCurrentParams());
    return this.http.get<IPaginate<IProduct>>(`${this.baseUrl}/Catalog`, {
      params
    });
  }

  getAllTypes() {
    return this.http.get<IType[]>(`${this.baseUrl}/Catalog/GetAllTypes`).pipe(map((x) => [{ id: '', name: 'All' }, ...x]));
  }

  getAllBrands() {
    return this.http.get<IBrand[]>(`${this.baseUrl}/Catalog/GetAllBrands`).pipe(map((x) => [{ id: '', name: 'All' }, ...x]));
  }

  private generateCatalogParams(catalogParams: ProductParams) {
    let params = new HttpParams();
    if (catalogParams.brandId) {
      params = params.append('brandId', catalogParams.brandId);
    }
    if (catalogParams.typeId) {
      params = params.append('typeId', catalogParams.typeId);
    }
    if (catalogParams.search) {
      params = params.append('search', catalogParams.search);
    }
    if (catalogParams.sort == 'priceAsc' || catalogParams.sort == 'priceDesc') {
      params = params.append('sort', catalogParams.sort);
    }
    params = params.append('pageIndex', catalogParams.pageIndex);
    params = params.append('pageSize', catalogParams.pageSize);
    return params;
  }
}
