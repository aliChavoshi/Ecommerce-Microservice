import { routes } from './../app.routes';
import { Component, OnInit } from '@angular/core';
import { StoreService } from './store.service';
import { Brand, Catalog, CatalogParams, Type } from '../shared/models/Catalog';
import { ProductItemComponent } from './product-item/product-item.component';
import { ShowTypesComponent } from './show-types/show-types.component';
import { ShowBrandsComponent } from './show-brands/show-brands.component';
import { ActivatedRoute, Router } from '@angular/router';
import { SortingComponent } from './sorting/sorting.component';
import { IPaginate } from '../shared/models/IPaginate';
import { PageChangedEvent, PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
  selector: 'app-store',
  imports: [ProductItemComponent, ShowTypesComponent, ShowBrandsComponent, SortingComponent, PaginationModule],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent implements OnInit {
  paginationProducts!: IPaginate<Catalog>;
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
    this.params.search = '';
    this.getAllProducts();
  }
  changeSorting(sort: any) {
    this.params.sort = sort;
    this.getAllProducts();
  }
  changedType(type: Type) {
    this.params.typeId = type.id;
    this.getAllProducts();
  }
  onPageChanged(event: PageChangedEvent) {
    this.params.pageIndex = event.page;
    this.getAllProducts();
  }
  changedBrand(brand: Brand) {
    this.params.brandId = brand.id;
    this.getAllProducts();
  }
  calculateEndPagination() {
    return this.params.pageSize * this.params.pageIndex > this.paginationProducts?.count ? this.paginationProducts?.count : this.params.pageSize * this.params.pageIndex;
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
      this.paginationProducts = res;
    });
  }
}
