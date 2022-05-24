import { ConnectionType } from "../constants/connection-type.enum";
import { UserActivity } from "./user-activity.dto";

export class User {

  public id?: string;

  public name?: string;

  public static asConnected(user : User): UserActivity {

    return {
      user: user,
      type: ConnectionType.Connection
    }
  }

  public static asDisconnected(user: User): UserActivity {

    return {
      user: user,
      type: ConnectionType.Disconnection
    }
  }
}
