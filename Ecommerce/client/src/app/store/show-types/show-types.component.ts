import { Component, OnInit } from '@angular/core';
import { StoreService } from '../store.service';
import { Type } from '../../shared/models/Catalog';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-show-types',
  imports: [CommonModule],
  templateUrl: './show-types.component.html',
  styleUrl: '../store.component.css'
})
export class ShowTypesComponent implements OnInit {
  types: Type[] = [];
  selectedItem?: Type;
  //
  constructor(private storeService: StoreService) {}
  selectItem(id: string) {
    this.selectedItem = this.types.find((x) => x.id == id);
  }
  ngOnInit(): void {
    this.getAllTypes();
  }

  private getAllTypes() {
    this.storeService.getAllTypes().subscribe((res) => {
      this.types = res;
    });
  }
}
