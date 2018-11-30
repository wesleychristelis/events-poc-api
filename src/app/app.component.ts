import { Component } from '@angular/core'
import { TransportAdapterWebsocket } from './shared/injectable/transport-adapter/transport-adapter-websocket.injectable'

@Component({
  selector: 'app-root',
  styleUrls: ['./app.component.css'],
  templateUrl: './app.component.html'
})
export class AppComponent {
  // Constructor
  constructor(private _transportAdapterWebsocket: TransportAdapterWebsocket) {}
}
