# Person profile cloud

Each bounded context has a separate project for the Azure functions it needs to run as a distributed application.
These functions are responsible for:

* Receiving the relevant statements from the perception event grid topic
* Instantiating the necesary dependencies
* Calling the application service MakeDecisionAsync function with the necesary dependencies
* Pushing the resulting statements back to the decision event grid topic
* Additional functions can be added for managing life-cycle concerns like database collection creation (which is not possible with ARM templates)

These functions should have very little variation as the business logic is always delegated, and can almost certainly be standardized.

There is an ARM template for the functions for each bounded context which is deployed as part of the release process.