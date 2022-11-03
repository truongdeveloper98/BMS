import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListPartnerCusComponent } from './list-partner-cus.component';

describe('ListPartnerCusComponent', () => {
  let component: ListPartnerCusComponent;
  let fixture: ComponentFixture<ListPartnerCusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListPartnerCusComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListPartnerCusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
