import { Injectable } from '@angular/core'
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr'
import { environment } from '../../../../environments/environment'
import { IStatement } from '../../xapi/statement.interface'
import { DecisionChannel, PerceptionChannel } from '../channel/channel.injectable'

@Injectable()
export class TransportAdapterWebsocket {
    private _connection: HubConnection
    private _connected = false
    private _pending: IStatement[] = []

    // Constructor
    constructor(private _decisionChannel: DecisionChannel, private _perceptionChannel: PerceptionChannel) {
        this._connection = new HubConnectionBuilder().withUrl(environment.webSocketHubUrl).build()

        this._connection.start().then(() => {
            this._connected = true
            this._pending.forEach(statement => this.send(statement))
        }, (err) => {
            console.error(err.toString())
            this._connected = false
        })

        this._perceptionChannel.observe().forEach((statement: IStatement) => {
            this._connected ? this.send(statement) : this._pending.push(statement)
        })

        this._connection.on('Decision', (message: IStatement) => {
            this._decisionChannel.next(message)
        })
    }

    // Public
    public send(statement: IStatement) {
        this._connection.send('Perception', statement)
    }
}
