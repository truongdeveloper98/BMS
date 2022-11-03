import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UseronboardListComponent } from './useronboard-list.component';

describe('UseronboardListComponent', () => {
  let component: UseronboardListComponent;
  let fixture: ComponentFixture<UseronboardListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UseronboardListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UseronboardListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
