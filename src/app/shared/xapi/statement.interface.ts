export interface IActivity {
    id: string
    definition?: IActivityDefinition
}

export interface IActivityDefinition {
    extensions?: any
}

export interface IActor {
    mbox: string
    name: string
    objectType: string
}

export interface IContext {
    statement: IStatementReference
}

export interface IStatementReference {
    id: string
}

export interface IStatement {
    actor: IActor
    context?: IContext
    object: IActivity
    verb: IVerb
}

export interface IVerb {
    id: string
}
