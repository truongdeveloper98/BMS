import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UseronboardComponent } from './useronboard.component';

describe('UseronboardComponent', () => {
  let component: UseronboardComponent;
  let fixture: ComponentFixture<UseronboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UseronboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UseronboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
