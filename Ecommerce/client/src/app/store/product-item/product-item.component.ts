import { Component, Input, OnInit, Pipe } from '@angular/core';
import { Catalog } from '../../shared/models/Catalog';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { StoreService } from '../store.service';

@Component({
  selector: 'app-product-item',
  imports: [CurrencyPipe, CommonModule, RouterModule],
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.css', '../store.component.css']
})
export class ProductItemComponent  {
  @Input({ required: true }) product!: Catalog;

}
