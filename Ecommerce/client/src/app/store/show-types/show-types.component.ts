import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
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
  selectedItem?: Type = { id: '', name: '' };
  @Output() selectedType = new EventEmitter<Type>();
  //
  constructor(private storeService: StoreService) {}

  selectItem(id: string) {
    this.selectedItem = this.types.find((x) => x.id == id);
    this.selectedType.emit(this.selectedItem);
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
