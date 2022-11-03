import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TodayReportComponent } from './today-report.component';

describe('TodayReportComponent', () => {
  let component: TodayReportComponent;
  let fixture: ComponentFixture<TodayReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TodayReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TodayReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
