# Person profile context

Each bounded context has a separate project for the domain, application, and framework layers.
This 'business logic' can be referenced either by the gateway (when deployed as a monolith) or by azure functions (when run distributed).
Either way, a component - for example the PersonComponent Azure function - is responsible for:

* Receiving the relevant statements from the perception channel
* Instantiating the necesary dependencies
* Calling the application service MakeDecisionAsync function with the necesary dependencies
* Pushing the resulting statements back to the decision channel

This business logic is the functional core of the application and should be thoroughly test covered. 
The projects should have as few dependencies as possible.