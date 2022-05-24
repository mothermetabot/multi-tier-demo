import { Observable, from } from "rxjs";
import { UserActivity } from "../Models/user-activity.dto";
import { UserMessage } from "../Models/user-message.dto";
import { UserModel } from "../Models/user.model";
import { RealTimeService } from "./real-time.service";

export class MockRealTimeService implements RealTimeService {
  onclosed(closed: () => void): void {
      throw new Error("Method not implemented.");
  }

  get id(): string {
      throw new Error("Method not implemented.");
  }
  connect(): Promise<void> {
      throw new Error("Method not implemented.");
  }

  sendUserMessage(message: UserMessage): Promise<boolean> {
      throw new Error("Method not implemented.");
  }


  subscribeToUserActivity(): Observable<UserActivity> {

    let users  : UserActivity[] = [{
        user : { name: "Joe", id: "1" },
        type : 0
      },
      {
        user: { name: "Peter", id: "2" },
        type: 0
      },
      {
        user: { name: "Ana", id: "3" },
        type: 0
      }
    ];

    return from(users);
  }
  subscribeToUserMessage(): Observable<UserMessage> {
    let messages = [
      new UserMessage("3", "Hallo")
    ];

    return from(messages);
  }

}
