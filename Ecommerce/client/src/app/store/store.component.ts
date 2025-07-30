import { Component, OnInit } from '@angular/core';
import { StoreService } from './store.service';
import { Brand, Catalog, CatalogParams, Type } from '../shared/models/Catalog';
import { ProductItemComponent } from './product-item/product-item.component';
import { ShowTypesComponent } from './show-types/show-types.component';
import { ShowBrandsComponent } from './show-brands/show-brands.component';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-store',
  imports: [ProductItemComponent, ShowTypesComponent, ShowBrandsComponent],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent implements OnInit {
  products: Catalog[] = [];
  brands: Brand[] = [];
  params!: CatalogParams;
  //
  constructor(private storeService: StoreService) {}
  //
  ngOnInit(): void {
    this.subscribeParams();
    this.getAllProducts();
  }

  reset() {
    this.params = new CatalogParams();
    this.getAllProducts();
  }
  changedType(type: Type) {
    this.params.typeId = type.id;
    this.getAllProducts();
  }
  changedBrand(brand: Brand) {
    this.params.brandId = brand.id;
    this.getAllProducts();
  }
  //private methods
  private subscribeParams() {
    this.storeService.params$.subscribe((params) => {
      this.params = params;
    });
  }

  private getAllProducts() {
    this.storeService.setParams(this.params);
    this.storeService.getAllCatalogs().subscribe((res) => {
      this.products = res.data;
    });
  }
}
