import { TestBed } from '@angular/core/testing';

import { SignlrServiceService } from './signlr-service.service';

describe('SignlrServiceService', () => {
  let service: SignlrServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SignlrServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
    
  });
});
