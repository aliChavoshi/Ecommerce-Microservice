import { Component, Input, Pipe } from '@angular/core';
import { Catalog } from '../../shared/models/Catalog';
import { CommonModule, CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-product-item',
  imports: [CurrencyPipe,CommonModule],
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.css','../store.component.css']
})
export class ProductItemComponent {
  @Input({required : true }) product! : Catalog;
}
