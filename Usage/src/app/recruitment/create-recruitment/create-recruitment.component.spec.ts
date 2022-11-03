import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateRecruitmentComponent } from './create-recruitment.component';

describe('CreateRecruitmentComponent', () => {
  let component: CreateRecruitmentComponent;
  let fixture: ComponentFixture<CreateRecruitmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateRecruitmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateRecruitmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
