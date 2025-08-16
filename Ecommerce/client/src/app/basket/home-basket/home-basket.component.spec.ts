import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeBasketComponent } from './home-basket.component';

describe('HomeBasketComponent', () => {
  let component: HomeBasketComponent;
  let fixture: ComponentFixture<HomeBasketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeBasketComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomeBasketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
