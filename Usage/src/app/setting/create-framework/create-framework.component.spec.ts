import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateFrameworkComponent } from './create-framework.component';

describe('CreateFrameworkComponent', () => {
  let component: CreateFrameworkComponent;
  let fixture: ComponentFixture<CreateFrameworkComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateFrameworkComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateFrameworkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
