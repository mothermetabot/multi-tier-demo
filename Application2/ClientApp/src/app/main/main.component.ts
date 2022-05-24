import { Component, Inject, OnInit } from '@angular/core';
import { User } from '../Models/user.dto';
import { filter } from "rxjs/operators";
import { REAL_TIME_SERVICE } from '../constants/di-tokens.constants';
import { UserActivity } from '../Models/user-activity.dto';
import { UserMessage } from '../Models/user-message.dto';
import { UserModel } from '../Models/user.model';
import { RealTimeService } from '../services/real-time.service';
import { CONNECTION_ACTIVITY, DISCONNECTION_ACTIVITY } from '../constants/server.constants';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  constructor(@Inject(REAL_TIME_SERVICE) private realTimeService: RealTimeService) {

    this.realTimeService.connect()
      .then(() => this.onConnected(), (e: any) => this.onNotConnected(e))
      .catch(console.log);
  }


  public users: UserModel[] = [];

  public selectedUser?: UserModel;

  public isConnected: boolean = false;


  ngOnInit(): void {


  }

  onSelectedUser(user: UserModel): void {
    user.hasNewMessage = false;
    this.selectedUser = user;
    console.log(user);
  }

  onInputChanged(event: any) {

    if (this.selectedUser?.id == null) {
      console.log("Selected user was null.");
      return;
    }

    let text = event.target.value;

    let message = UserMessage.fromText(this.selectedUser.id, text);

    this.realTimeService.sendUserMessage(message)
      .catch((e: Error) => console.log(e));
  }

  private onUserConnection(user: User) {

    let newUserModel = new UserModel(user.name, user.id);

    // if user already exists log and return
    if (this.users.some(model => model.id == user.id)) {
      console.log("An user with id: " + user.id + " is already connected.");
      return;
    }

    // no user present, so add user and make selected user.
    if (this.users.length == 0)
      this.selectedUser = newUserModel;

    this.users.push(newUserModel)

    console.log("user connection: ")
    console.log(user);
  }

  private onUserDisconnection(user: User) {

    let userToDelete = this.users.find(model => model.id == user.id);

    // user doesn't exist
    if (userToDelete == null) {
      console.log("Tried to delete user that was found in users collection. Id " + user.id);
      return;
    }

    let index = this.users.indexOf(userToDelete); //find index in your array
    this.users.splice(index, 1);//remove element from array

    console.log("user disconnection: ")
    console.log(user);

    // if the disconnected user is the one curretnly selected we need to either find a new one to select or set it to null.
    if (this.selectedUser !== userToDelete) return;

    if (this.users.length == 0)
      this.selectedUser == null;

    else this.selectedUser = this.users[0];
  }

  private startConnection(): void {
    this.isConnected = false;
    this.realTimeService.connect()
      .then(() => this.onConnected(), (e: any) => this.onNotConnected(e))
      .catch(console.log);
  }

  private onNotConnected(e: any): void {
    console.log("failed to connect");
    console.log(e);
  }

  private onConnected(): void {

    this.realTimeService.onclosed(() => this.startConnection());

    console.log("Connection to real-time server established!");

    this.realTimeService.subscribeToUserActivity()
      .subscribe({

        next: (activity: UserActivity) => {
          if (activity.user == null)
            return;

          if (activity.user.id == this.realTimeService.id) {
            console.log("Received user actitivy of self... Ignoring entry.");
            return;
          }

          if (activity.type == CONNECTION_ACTIVITY)
            this.onUserConnection(activity.user);

          else if (activity.type == DISCONNECTION_ACTIVITY)
            this.onUserDisconnection(activity.user);
        },

        error: (e: Error) => console.log(e),


        complete: () => { }
      });


    this.realTimeService.subscribeToUserMessage()
      .subscribe({

        next: (m: UserMessage) => {

          console.log("Received user message.");

          if (this.users.length == 0) {
            console.log("Users collection was empty or ");
            return;
          }
          else if (this.selectedUser == null)
            this.selectedUser = this.users[0];

          let sender = this.users
            .find(user => user.id === m.userId);

          if (sender == null) {
            console.log("Couldn't find sender of message in users collection.");
            return;
          }

          if (sender != this.selectedUser)
            this.selectedUser.hasNewMessage = true;

          sender.receivedText = m.content;
        },

        error: (e: Error) => console.log(e),

        complete: () => { }
      });

    this.isConnected = true;
  }

}
