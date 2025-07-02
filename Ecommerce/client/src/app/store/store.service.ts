import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Catalog } from '../shared/models/Catalog';
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
}
