import { Observable } from 'rxjs';
import { UserActivity } from '../Models/user-activity.dto';
import { UserMessage } from '../Models/user-message.dto';

export interface RealTimeService {

  get id(): string;

  onclosed(closed: () => void): void;

  subscribeToUserActivity(): Observable<UserActivity>;

  subscribeToUserMessage(): Observable<UserMessage>;

  sendUserMessage(message: UserMessage): Promise<boolean>;

  connect(): Promise<void>;
}
