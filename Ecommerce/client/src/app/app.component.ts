import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './core/navbar/navbar.component';
import { HttpClient } from '@angular/common/http';
import { IPaginate } from './shared/models/IPaginate';
import { Catalog } from './shared/models/Catalog';
import { StoreComponent } from "./store/store.component";
import { FooterComponent } from "./core/footer/footer.component";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, StoreComponent, FooterComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client';

}
