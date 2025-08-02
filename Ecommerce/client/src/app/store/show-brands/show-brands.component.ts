import { Component, EventEmitter, OnInit, Output, output } from '@angular/core';
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
  selectedItem?: Brand = { id: '', name: '' };
  @Output() selectedBrand = new EventEmitter<Brand>();
  //
  selectItem(id: string) {
    this.selectedItem = this.brands.find((x) => x.id == id);
    this.selectedBrand.emit(this.selectedItem);
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
