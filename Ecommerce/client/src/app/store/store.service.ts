import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Brand, Catalog, Type } from '../shared/models/Catalog';
import { IPaginate } from '../shared/models/IPaginate';

@Injectable({
  providedIn: 'root'
})
export class StoreService {
  private baseUrl = 'http://localhost:9010';
  constructor(private http: HttpClient) {

  }

  getAllCatalogs() {
    return this.http.get<IPaginate<Catalog>>(`${this.baseUrl}/Catalog`);
  }

  getAllTypes(){
    return this.http.get<Type[]>(`${this.baseUrl}/Catalog/GetAllTypes`);
  }

  getAllBrands(){
    return this.http.get<Brand[]>(`${this.baseUrl}/Catalog/GetAllBrands`);
  }
}
