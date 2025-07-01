import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './core/navbar/navbar.component';
import { HttpClient } from '@angular/common/http';
import { IPaginate } from './shared/models/IPaginate';
import { Catalog } from './shared/models/Catalog';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client';
  constructor(private http: HttpClient) {
    this.http.get<IPaginate<Catalog>>('http://localhost:9010/Catalog').subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.error(error);
      },
      complete: () => {
        console.log('Request complete');
      }
    });
  }
}
