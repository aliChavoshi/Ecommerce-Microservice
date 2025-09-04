import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './core/navbar/navbar.component';
import { FooterComponent } from './core/footer/footer.component';
import { HeaderComponent } from './core/header/header.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, FooterComponent, HeaderComponent, NgxSpinnerModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class AppComponent implements OnInit {
  constructor(private basketService : BasketService) {}
  ngOnInit(): void {
    const loggedUser = localStorage.getItem('basket_username');
    if(loggedUser){
      this.basketService.getBasket(loggedUser).subscribe(res=>{
      // console.log("🚀 ~ AppComponent ~ ngOnInit ~ res:", res)
      });
    }
  }
}
