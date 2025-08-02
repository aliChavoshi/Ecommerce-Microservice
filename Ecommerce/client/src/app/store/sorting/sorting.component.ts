import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { StoreService } from '../store.service';
import { CatalogParams } from '../../shared/models/Catalog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sorting',
  imports: [CommonModule],
  templateUrl: './sorting.component.html',
  styleUrl: '../store.component.css'
})
export class SortingComponent implements OnInit {
  sortOptions = [
    {
      name: 'Title',
      value: 'title'
    },
    {
      name: 'Price: Descending',
      value: 'priceDesc'
    },
    {
      name: 'Price: Ascending',
      value: 'priceAsc'
    }
  ];
  params!: CatalogParams;
  selectedItem = this.params?.sort ?? 'priceAsc';
  @Output() sortingValue = new EventEmitter<string>();
  //
  constructor(private storeService: StoreService) {}
  ngOnInit(): void {
    this.getParams();
  }
  changeSorting(sort: any) {
    this.selectedItem = sort.value;
    this.sortingValue.emit(this.selectedItem);
  }
  private getParams() {
    this.storeService.params$.subscribe((res) => {
      this.params = res;
    });
  }
}
