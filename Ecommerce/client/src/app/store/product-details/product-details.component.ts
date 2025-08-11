import { Component } from '@angular/core';
import { StoreService } from '../store.service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Catalog } from '../../shared/models/Catalog';
import { DecimalPipe } from '@angular/common';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-product-details',
  imports: [RouterLink,DecimalPipe],
  templateUrl: './product-details.component.html',
  styleUrl: './product-details.component.css'
})
export class ProductDetailsComponent {
  private productId!: string;
  product! : Catalog;
  constructor(private storeService: StoreService, private route: ActivatedRoute,private bcService : BreadcrumbService) {
    this.productId = this.route.snapshot.params['id'];
  }
  ngOnInit(): void {
    this.getProductById();
  }

  private getProductById() {
    this.storeService.getProductById(this.productId).subscribe((res) => {
      this.product = res;
      this.bcService.set('@productDetail', this.product.name);
    });
  }
}
