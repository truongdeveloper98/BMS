import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PendingPopupComponent } from './pending-popup.component';

describe('PendingPopupComponent', () => {
  let component: PendingPopupComponent;
  let fixture: ComponentFixture<PendingPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PendingPopupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PendingPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
