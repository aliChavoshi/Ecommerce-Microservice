import { Component } from '@angular/core';
import { PageChangedEvent, PaginationModule } from 'ngx-bootstrap/pagination';
import { Brand, Catalog, CatalogParams, Type } from '../../shared/models/Catalog';
import { IPaginate } from '../../shared/models/IPaginate';
import { StoreService } from '../store.service';
import { ProductItemComponent } from '../product-item/product-item.component';
import { RouterModule } from '@angular/router';
import { ShowTypesComponent } from '../show-types/show-types.component';
import { ShowBrandsComponent } from '../show-brands/show-brands.component';
import { SortingComponent } from '../sorting/sorting.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-home-store',
  imports: [ProductItemComponent, PaginationModule, RouterModule, ShowTypesComponent, ShowBrandsComponent, SortingComponent, FormsModule],
  templateUrl: './home-store.component.html',
  styleUrl: './home-store.component.css'
})
export class HomeStoreComponent {
  paginationProducts!: IPaginate<Catalog>;
  brands: Brand[] = [];
  searchInput = '';
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
    this.searchInput = '';
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
  search() {
    this.params.search = this.searchInput;
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
      this.paginationProducts = res;
    });
  }
}
