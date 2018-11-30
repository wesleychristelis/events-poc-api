import { Injectable } from '@angular/core'
import { Observable, Subject } from 'rxjs'
import { filter, map } from 'rxjs/operators'
import { clone } from '../../utility/utility'
import { IStatement } from '../../xapi/statement.interface'

@Injectable()
export class Channel {
    protected _channelName: string
    private _subject: Subject<IStatement>

    // Constructor
    constructor() {
        this._subject = new Subject()
    }

    // Public
    next(statement: IStatement) {
        console.log(this._channelName + ' channel: ' + JSON.stringify(statement))
        this._subject.next(statement)
    }

    observe(): Observable<IStatement> {
        return this._subject.pipe(
            map((statement: IStatement) => clone(statement)))
    }

    observeVerbs(verbs: string[]): Observable<IStatement> {
        return this.observe().pipe(
            filter((statement: IStatement) => verbs.includes(statement.verb.id) ))
    }
}

export class DecisionChannel extends Channel {
    _channelName = 'Decision'
}
export class PerceptionChannel extends Channel {
    _channelName = 'Perception'
}
