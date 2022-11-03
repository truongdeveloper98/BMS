import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateOffReportComponent } from './create-off-report.component';

describe('CreateOffReportComponent', () => {
  let component: CreateOffReportComponent;
  let fixture: ComponentFixture<CreateOffReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateOffReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateOffReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
