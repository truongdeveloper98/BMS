import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ListReportGennericComponent } from './list-report-genneric.component';

describe('ListReportGennericComponent', () => {
  let component: ListReportGennericComponent;
  let fixture: ComponentFixture<ListReportGennericComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListReportGennericComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ListReportGennericComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
