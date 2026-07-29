import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AreaAnalysis } from './area-analysis';

describe('AreaAnalysis', () => {
  let component: AreaAnalysis;
  let fixture: ComponentFixture<AreaAnalysis>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AreaAnalysis],
    }).compileComponents();

    fixture = TestBed.createComponent(AreaAnalysis);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
