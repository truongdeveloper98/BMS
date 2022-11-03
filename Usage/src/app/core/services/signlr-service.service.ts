import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SignlrServiceService {
  constructor() {}

  hubConnection!: signalR.HubConnection;

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrlRoot + '/toastr', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();


    this.hubConnection
      .start()
      .then(() => {
      })
      .catch((err) => console.log('Error while starting connection: ' + err));
  };

  askServer(userName : String, role : String) {
    this.hubConnection
      .invoke('askServer', userName, role)
      .catch((err) => console.error(err));
  }

  askServerListener() {
    this.hubConnection.on('askServerResponse', (someText) => {
      alert(someText);
    });
  }
}
