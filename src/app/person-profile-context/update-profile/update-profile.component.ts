import { Component, Version } from '@angular/core'
import { DecisionChannel, PerceptionChannel } from '../../shared/injectable/channel/channel.injectable'
import { StatementFactory } from '../../shared/injectable/statement-factory/statement-factory.injectable'
import { IStatement } from '../../shared/xapi/statement.interface'
import { Verb } from '../../shared/xapi/verb.enum'
import { Person } from './update-profile.class'

@Component({
    selector: 'app-update-profile',
    templateUrl: './update-profile.component.html'
})
export class UpdateProfileComponent {
    person: Person = new Person()

    // Constructor
    constructor(private _channelDecision: DecisionChannel,
        private _channelPerception: PerceptionChannel,
        private _statementFactory: StatementFactory
    ) {
        const personChangedVerbs = [Verb.PersonCreated, Verb.PersonRetrieved, Verb.PersonUpdated]
        this._channelDecision.observeVerbs(personChangedVerbs).forEach((statement: IStatement) => {
            this.person = this._statementFactory.extractData(statement)
            console.log('Person updated')
        })

        const failureVerbs = [Verb.PersonCreationFailed, Verb.PersonUpdateFailed]
        this._channelDecision.observeVerbs([Verb.PersonCreationFailed]).forEach((statement: IStatement) => {
            alert('There was an error saving your information.')
        })
    }

    // Public
    send() {
        const statement = this.person.id ?
            this._statementFactory.create(this.person.id, Verb.PersonUpdateRequested, this.person) :
            this._statementFactory.create('http://eventuality.poc/person', Verb.PersonCreationRequested, this.person)

        this._channelPerception.next(statement)
    }
}
