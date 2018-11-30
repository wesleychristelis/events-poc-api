# Gateway

The role of the gateway is to:

* Manage all transport to / from clients
* Forward statements coming in on websocket connections to the perception channel
* Forward statements coming in on the decision channel to the appropriate websocket connection

### Running as a monolith

When running as a monolith, all business logic also runs within the gateway, with component to bridge between the channels and business logic.
In this case these components are added as part of the startup cycle.

### Running as a distributed app

When running distributed, an adapter passes all perception channel statements on to the perception event grid topic, where they can be picked up and processed.
A controller receives calls from the decision event grid topic subscription and forwards them on to the internal decision channel.

### ARM template

There is an ARM template for the gateway which is deployed as part of the release process.
The database and event grid templates should be deployed once when a new environment is set up.