import { HubConnectionBuilder } from "@microsoft/signalr";
import { environment } from "../../environments/environment";
import { NAME_QUERY_KEY } from "../constants/server.constants";

export const hubConnectionFactory = () => {

  let base = environment.serviceUrl;
  let complete = base + "?" + NAME_QUERY_KEY + "=admin";

  console.log("# building hub connection to path: " + complete);

  return new HubConnectionBuilder()
    .withUrl(complete)
    .withAutomaticReconnect()
    .build();
}
