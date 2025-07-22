import { TestBed } from '@angular/core/testing';

import { Unit } from './unit';

describe('Unit', () => {
  let service: Unit;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Unit);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
