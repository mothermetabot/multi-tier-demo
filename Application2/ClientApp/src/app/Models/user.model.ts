export class UserModel {

  constructor(public name?: string,
    public id?: string,
    public receivedText: string = "",
    public sentText : string = "",
    public hasNewMessage: boolean = false  ) {

  }
}
