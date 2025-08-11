import { Component } from '@angular/core';
import { BreadcrumbComponent, BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-header',
  imports: [BreadcrumbComponent],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  showBreadcrumbs: boolean = true;

  constructor(private bcService: BreadcrumbService) {
    this.bcService.breadcrumbs$.subscribe((breadcrumbs) => {
      if (breadcrumbs.some((b) => b.label === 'Home') && breadcrumbs.length === 1) {
        this.showBreadcrumbs = false;
        return;
      }
      this.showBreadcrumbs = true;
    });
  }
}
