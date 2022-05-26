import { TestBed } from '@angular/core/testing';
import { HubConnection, HubConnectionState } from '@microsoft/signalr';
import { ConnectionType } from '../constants/connection-type.enum';
import { User } from '../Models/user.dto';

import { SignalrRealTimeService } from './signalr.real-time.service';

describe('SignalrRealTimeService', () => {

  let service: SignalrRealTimeService;
  let mockService: jasmine.SpyObj<HubConnection>;


  beforeEach(() => {

    const spy = jasmine.createSpyObj('hubConnection', ['start', 'invoke', 'on'], ['state']);

    TestBed.configureTestingModule({
      providers: [SignalrRealTimeService,
        {
          provide: HubConnection,
          useValue: spy
        }]
    });

    // Inject both the service-to-test and its (spy) dependency
    service = TestBed.inject(SignalrRealTimeService);
    mockService = TestBed.inject(HubConnection) as jasmine.SpyObj<HubConnection>;
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return current users', () => {

    let stubUsers: User[] = [
      {
        name: "1",
        id: "1"
      },
      {
        name: "2",
        id: "2"
      }];

    mockService.invoke.and.returnValue(new Promise((resolve) => {
      resolve(stubUsers);
    }));

    (Object.getOwnPropertyDescriptor(mockService, 'state')?.get as jasmine.Spy<() => HubConnectionState>)
      .and.returnValue(HubConnectionState.Connected);

    let observable = service.subscribeToUserActivity()
      .subscribe({
        next: activity => {

          if (activity.user == null) {
            expect(activity.user).toBeDefined;
            return;
          }

          expect(stubUsers).toContain(activity.user);
          expect(activity.type)
            .toBe(ConnectionType.Connection);
        }
      });


    expect(observable)
      .toBeTruthy();
  })

  it('should throw error for connection closed', () => {
    let stubUsers: User[] = [
      {
        name: "1",
        id: "1"
      },
      {
        name: "2",
        id: "2"
      }];

    mockService.invoke.and.returnValue(new Promise(r => r(null)));

    (Object.getOwnPropertyDescriptor(mockService, 'state')?.get as jasmine.Spy<() => HubConnectionState>)
      .and.returnValue(HubConnectionState.Connected);

    expect(service.subscribeToUserActivity)
      .toThrowError();
  })
});
