import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckReportComponent } from './check-report.component';

describe('CheckReportComponent', () => {
  let component: CheckReportComponent;
  let fixture: ComponentFixture<CheckReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
