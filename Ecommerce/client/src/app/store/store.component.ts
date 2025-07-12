import { Component, OnInit } from '@angular/core';
import { StoreService } from './store.service';
import { Catalog } from '../shared/models/Catalog';
import { ProductItemComponent } from "./product-item/product-item.component";

@Component({
  selector: 'app-store',
  imports: [ProductItemComponent],
  templateUrl: './store.component.html',
  styleUrl: './store.component.css'
})
export class StoreComponent implements OnInit {
  //properties
  products: Catalog[] = [];
  //
  constructor(private storeService: StoreService) {}
  //
  ngOnInit(): void {
    this.getAllProducts();
  }
  //private methods
  private getAllProducts() {
    this.storeService.getAllCatalogs().subscribe((res) => {
      this.products = res.data;
    });
  }

}
