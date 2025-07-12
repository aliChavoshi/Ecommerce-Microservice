import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Brand, Catalog, CatalogParams, Type } from '../shared/models/Catalog';
import { IPaginate } from '../shared/models/IPaginate';

@Injectable({
  providedIn: 'root'
})
export class StoreService {
  private baseUrl = 'http://localhost:9010';
  constructor(private http: HttpClient) {}

  getAllCatalogs(catalogParams: CatalogParams) {
    let params = this.generateCatalogParams(catalogParams);
    return this.http.get<IPaginate<Catalog>>(`${this.baseUrl}/Catalog`, {
      params
    });
  }

  getAllTypes() {
    return this.http.get<Type[]>(`${this.baseUrl}/Catalog/GetAllTypes`);
  }

  getAllBrands() {
    return this.http.get<Brand[]>(`${this.baseUrl}/Catalog/GetAllBrands`);
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
