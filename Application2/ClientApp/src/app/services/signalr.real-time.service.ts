import { Observable, Subject } from "rxjs";
import { UserMessage } from "../Models/user-message.dto";
import { RealTimeService } from "./real-time.service";
import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { NAME_QUERY_KEY } from "../constants/server.constants";
import { RECEIVE_USER_ACTIVITY, RECEIVE_BROADCAST, SEND_MESSAGE, GET_USERS } from "../constants/method.constants";
import { UserActivity } from "../Models/user-activity.dto";
import { User } from "../Models/user.dto";
import { environment } from '../../environments/environment';

export class SignalrRealTimeService implements RealTimeService {

  constructor() {

    let base = environment.serviceUrl;
    let url = base + "?" + NAME_QUERY_KEY + "=admin";

    console.log("Attempting to connect to real-time service under " + url);

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(url)
      .withAutomaticReconnect()
      .build();
  }

  onclosed(closed: () => void): void {
    this.hubConnection.onclose(() => closed());
  }

  get id(): string {
    if (this.hubConnection.connectionId == null)
      throw new Error("The signalR server is not connected as such, there is no connection id to return.");

    return this.hubConnection.connectionId;
  }

  private hubConnection: HubConnection;

  private userActivitySubject: Subject<UserActivity> = new Subject<UserActivity>();

  private userMessageSubject: Subject<UserMessage> = new Subject<UserMessage>();

  connect(): Promise<void> {
    return this.hubConnection.start();
  }

  private getCurrentUsers(): Promise<User[]> {
    if (this.hubConnection.state != HubConnectionState.Connected)
      throw new Error("HubConnection is closed");

    return this.hubConnection.invoke<User[]>(GET_USERS);
  }

  subscribeToUserActivity(): Observable<UserActivity> {

    // retreive current users and push them to the subject
    // this is to avoid inconsistencies regarding users that were logged in before we connected.
    this.getCurrentUsers()
      .then((users) =>

        users.forEach(user => {
          let activity = User.asConnected(user);
          this.userActivitySubject.next(activity);
        }))
      .catch((e: Error) => console.log(e));

    this.hubConnection.on(RECEIVE_USER_ACTIVITY,
      (activity: UserActivity) => this.userActivitySubject.next(activity))

    // it is a best practice to generate a new observable,
    // instead of returning the subject as an observable.
    return new Observable(subscriber => {
      this.userActivitySubject.subscribe(subscriber)
    });
  }


  subscribeToUserMessage(): Observable<UserMessage> {
    this.hubConnection.on(RECEIVE_BROADCAST,
      (message: UserMessage) => this.userMessageSubject.next(message))

    return new Observable(subscriber => {
      this.userMessageSubject.subscribe(subscriber)
    });
  }


  sendUserMessage(message: UserMessage): Promise<boolean> {
    let promise = this.hubConnection.invoke(SEND_MESSAGE, message)
      .then(() => true, () => false);

    return promise;
  }
}
