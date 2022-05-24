export class UserMessage {
  constructor(public userId: string, public content: string) {

  }

  public static fromText(id: string, content: string = ''): UserMessage {

    return {
      userId: id,
      content: content
    };
  }
}

