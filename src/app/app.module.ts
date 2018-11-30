import { NgModule } from '@angular/core'
import { FormsModule } from '@angular/forms'
import { BrowserModule } from '@angular/platform-browser'
import { AppComponent } from './app.component'
import { UpdateProfileComponent } from './person-profile-context/update-profile/update-profile.component'
import { DecisionChannel, PerceptionChannel } from './shared/injectable/channel/channel.injectable'
import { StatementFactory } from './shared/injectable/statement-factory/statement-factory.injectable'
import { TransportAdapterWebsocket } from './shared/injectable/transport-adapter/transport-adapter-websocket.injectable'

@NgModule({
  bootstrap: [AppComponent],
  declarations: [
    AppComponent, UpdateProfileComponent
  ],
  imports: [
    BrowserModule, FormsModule
  ],
  providers: [
    DecisionChannel, PerceptionChannel,
    StatementFactory, TransportAdapterWebsocket
  ]
})
export class AppModule { }
