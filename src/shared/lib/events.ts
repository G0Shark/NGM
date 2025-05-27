//import { useServersStore } from "../model/store/servers";
import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
/**
 * @method ServerUpdate <---
 * 
 */
export function InitializeEventSystem(): HubConnection {
  const sys = new HubConnectionBuilder()
    .withUrl("")
    .configureLogging(LogLevel.Information)
    .build();
  sys.on('message', () => { });
  return sys;
}