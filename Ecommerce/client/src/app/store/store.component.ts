import { Component, OnInit } from '@angular/core';
import { StoreService } from './store.service';
import { Brand, Catalog, CatalogParams, Type } from '../shared/models/Catalog';
import { ProductItemComponent } from './product-item/product-item.component';
import { ShowTypesComponent } from './show-types/show-types.component';
import { ShowBrandsComponent } from './show-brands/show-brands.component';

@Component({
  selector: 'app-store',
  imports: [ProductItemComponent, ShowTypesComponent, ShowBrandsComponent],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent implements OnInit {
  //properties
  products: Catalog[] = [];
  brands: Brand[] = [];
  params = new CatalogParams();
  //
  constructor(private storeService: StoreService) {}
  //
  ngOnInit(): void {
    this.getAllProducts();
  }
  //private methods
  private getAllProducts() {
    this.storeService.getAllCatalogs(this.params).subscribe((res) => {
      this.products = res.data;
    });
  }
}
