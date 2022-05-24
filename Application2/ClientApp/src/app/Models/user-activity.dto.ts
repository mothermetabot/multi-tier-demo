import { ConnectionType } from "../constants/connection-type.enum";
import { User } from "./user.dto";

export class UserActivity {
  public type?: ConnectionType;

  public user?: User;
}
