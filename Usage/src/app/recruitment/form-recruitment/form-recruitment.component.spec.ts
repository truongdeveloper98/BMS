import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FormRecruitmentComponent } from './form-recruitment.component';

describe('FormRecruitmentComponent', () => {
  let component: FormRecruitmentComponent;
  let fixture: ComponentFixture<FormRecruitmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FormRecruitmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FormRecruitmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
