import { ComponentFixture, TestBed } from '@angular/core/testing';
import { REAL_TIME_SERVICE } from '../constants/di-tokens.constants';
import { SignalrRealTimeService } from '../services/signalr.real-time.service';

import { MainComponent } from './main.component';

describe('MainComponent', () => {
  let component: MainComponent;
  let fixture: ComponentFixture<MainComponent>;
  const mockSignalrService = jasmine.createSpyObj('SignalrRealTimeService', ['connect'],);

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      providers: [{
        provide: SignalrRealTimeService,
        useValue: mockSignalrService
      }],
      declarations: [MainComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MainComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
