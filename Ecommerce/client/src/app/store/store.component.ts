import { Component, OnInit } from '@angular/core';
import { StoreService } from './store.service';
import { Brand, Catalog, Type } from '../shared/models/Catalog';
import { ProductItemComponent } from './product-item/product-item.component';

@Component({
  selector: 'app-store',
  imports: [ProductItemComponent],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent implements OnInit {
  //properties
  products: Catalog[] = [];
  brands : Brand[] = [];
  types : Type[] = [];
  //
  constructor(private storeService: StoreService) {}
  //
  ngOnInit(): void {
    this.getAllProducts();
    this.getAllBrands();
    this.getAllTypes();
  }
  //private methods
  private getAllTypes() {
    this.storeService.getAllTypes().subscribe((res) => {
      this.types = res;
    });
  }

  private getAllBrands() {
    this.storeService.getAllBrands().subscribe((res) => {
      this.brands = res;
    });
  }

  private getAllProducts() {
    this.storeService.getAllCatalogs().subscribe((res) => {
      this.products = res.data;
    });
  }
}
