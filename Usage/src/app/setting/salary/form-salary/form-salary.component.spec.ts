import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormSalaryComponent } from './form-salary.component';

describe('FormSalaryComponent', () => {
  let component: FormSalaryComponent;
  let fixture: ComponentFixture<FormSalaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormSalaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormSalaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
