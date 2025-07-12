import { Component, OnInit } from '@angular/core';
import { StoreService } from '../store.service';
import { Brand } from '../../shared/models/Catalog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-show-brands',
  imports: [CommonModule],
  templateUrl: './show-brands.component.html',
  styleUrl: '../store.component.css'
})
export class ShowBrandsComponent implements OnInit {
  brands: Brand[] = [];
  selectedItem?: Brand;
  //
  selectItem(id: string) {
    this.selectedItem = this.brands.find((x) => x.id == id);
  }

  constructor(private storeService: StoreService) {}
  ngOnInit(): void {
    this.getAllBrands();
  }

  private getAllBrands() {
    this.storeService.getAllBrands().subscribe((res) => {
      this.brands = res;
    });
  }
}
