import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListReportOffComponent } from './list-report-off.component';

describe('ListReportOffComponent', () => {
  let component: ListReportOffComponent;
  let fixture: ComponentFixture<ListReportOffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListReportOffComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListReportOffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
