import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowTypesComponent } from './show-types.component';

describe('ShowTypesComponent', () => {
  let component: ShowTypesComponent;
  let fixture: ComponentFixture<ShowTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShowTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ShowTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
