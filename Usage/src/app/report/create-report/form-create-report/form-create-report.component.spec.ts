import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormCreateReportComponent } from './form-create-report.component';

describe('FormCreateReportComponent', () => {
  let component: FormCreateReportComponent;
  let fixture: ComponentFixture<FormCreateReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormCreateReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormCreateReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
