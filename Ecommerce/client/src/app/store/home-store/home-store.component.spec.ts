import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HomeStoreComponent } from './home-store.component';

describe('HomeStoreComponent', () => {
  let component: HomeStoreComponent;
  let fixture: ComponentFixture<HomeStoreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HomeStoreComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(HomeStoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
