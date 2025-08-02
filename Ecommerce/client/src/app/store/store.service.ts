import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Brand, Catalog, CatalogParams, Type } from '../shared/models/Catalog';
import { IPaginate } from '../shared/models/IPaginate';
import { BehaviorSubject, map, Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StoreService {
  private baseUrl = 'http://localhost:9010';
  private paramsSource = new BehaviorSubject<CatalogParams>(new CatalogParams());
  params$ = this.paramsSource.asObservable();

  constructor(private http: HttpClient) {}

  getCurrentParams() {
    return this.paramsSource.value;
  }

  setParams(params: CatalogParams) {
    this.paramsSource.next(params);
  }

  getAllCatalogs() {
    let params = this.generateCatalogParams(this.getCurrentParams());
    return this.http.get<IPaginate<Catalog>>(`${this.baseUrl}/Catalog`, {
      params
    });
  }

  getAllTypes() {
    return this.http.get<Type[]>(`${this.baseUrl}/Catalog/GetAllTypes`).pipe(map((x) => [{ id: '', name: 'All' }, ...x]));
  }

  getAllBrands() {
    return this.http.get<Brand[]>(`${this.baseUrl}/Catalog/GetAllBrands`).pipe(map((x) => [{ id: '', name: 'All' }, ...x]));
  }

  private generateCatalogParams(catalogParams: CatalogParams) {
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
