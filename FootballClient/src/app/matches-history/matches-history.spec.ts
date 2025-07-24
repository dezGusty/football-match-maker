import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchesHistory } from './matches-history';

describe('MatchesHistory', () => {
  let component: MatchesHistory;
  let fixture: ComponentFixture<MatchesHistory>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MatchesHistory]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MatchesHistory);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
