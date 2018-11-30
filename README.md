# Eventuality POC

## Description

This is a proof-of-concept for a [full-stack event-based architecture](https://medium.com/@matt.denobrega/state-vs-event-based-web-architectures-59ab1f47656b).
Everything from the UI to the persistence layer, and from low-level transport to business logic, works with normalized events.

This is also partially an exploration of how to develop, deploy, and manage a serverless web application, and as such is significantly more complex than justified by the scope. 
For more context on the design decisions and information flow, see [the medium article about this solution](https://medium.com/@matt.denobrega/639ebd017977).

The scope of this POC is extremely limited - creating and saving Person instances.

## Prerequisites

#### For local development

* [Visual studio](https://visualstudio.microsoft.com/vs/)
* [Azure Cosmos db emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)

#### To deploy to Azure

* An active Azure subscription (you can get a [free account](https://azure.microsoft.com/free/?WT.mc_id=A261C142F) if you don't have one).
* The Azure functions tools for Visual Studio - see [this article](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs).

## Getting Started

Clone the repository, open the solution in Visual studio, and run the Gateway project.
The gateway is set up to use RX channels and the CosmosDB emulator locally, so there are no cloud dependencies.
The best way to interact with the gateway is to run the [web app](https://github.com/matthewdenobrega/eventuality-poc-web).

## Project structure

The solution is designed to run both as an in-process monolith and as a distributed cloud app with as little code overhead as possible:

* The Context solution folder has a separate project for the business logic of each bounded context:
  * These cover the domain, application, and framework layers
  * These are supporting libraries and are not deployable
  * They depend only on the Shared project
* The Gateway project:
  * Primarily acts as an http-to-channel adapter
  * Also includes components which bridge the channels and business logic - these are needed if running as a monolith
  * The gateway project also includes ARM templates for the key vault, cosmos db, event grid, and web app
* The Cloud solution folder has a separate project for the Azure functions for each bounded context:
  * These functions bridge the channels and business logic, performing the same role as the components in the monolith
  * They depend only on the Context project of their bounded context
  * The ARM templates for the function web apps are stored here
* The Shared project includes core classes that are used across the other projects
* There are separate Test projects where needed

While this level of complexity is overkill for the very limited scope, this structure and deployment should scale to much larger applications.

## Running the tests

Run all the tests in the solution - the tests are written with MSTest and should be recognized automatically by Visual Studio.

## Deployment

The application is set up to run on event grid and functions on Azure.
The cloud deployment uses five resource groups, to separate out different resource life cycles - the buttons deploy the dev environment:

* One for the Key vault [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmatthewdenobrega%2Feventuality-poc-api%2Fmaster%2FGateway%2FInfrastructure%2FKeyVault%2Fazuredeploy.json)
* One for the CosmosDB [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmatthewdenobrega%2Feventuality-poc-api%2Fmaster%2FGateway%2FInfrastructure%2FDatabase%2Fazuredeploy.json)
* One for the Event grid [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmatthewdenobrega%2Feventuality-poc-api%2Fmaster%2FGateway%2FInfrastructure%2FEventGrid%2Fazuredeploy.json)
* One for the Gateway web app [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmatthewdenobrega%2Feventuality-poc-api%2Fmaster%2FGateway%2FInfrastructure%2FWebApp%2Fazuredeploy.json)
* One for the Azure functions for each bounded context [![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fmatthewdenobrega%2Feventuality-poc-api%2Fmaster%2FPersonProfileCloud%2FInfrastructure%2Fazuredeploy.json)

The event grid and functions resources are not needed if running as an in-process monolith. 
There are some dependencies between resources which need to be set in the key vault after the database and event grid are deployed:

* cosmosDBAccountEndpoint
* cosmosDBAccountKey
* eventGridPersonProfileContextDecisionTopicKey
* eventGridPersonProfileContextDecisionTopicUrl
* eventGridPersonProfileContextPerceptionTopicKey
* eventGridPersonProfileContextPerceptionTopicUrl

After deploying, publish the Gateway and PersonProfileCloud projects to their app services.

## Naming convention for azure resources

Expanded from [Microsoft's best practices](https://docs.microsoft.com/en-us/azure/architecture/best-practices/naming-conventions).

solution-rg-context?-aggregate?-description?-environment-resource

Were rg = resource group

* eventualitypoc-gateway-dev-rg (gateway dev resource group)
* eventualitypoc-gateway-dev-wa (gateway dev web app)
* eventualitypoc-eventgrid-ppc-perception-dev-egt (dev event grid person profile context perception topic)
* eventualitypoc-cloud-ppc-person-dev-func (dev person profile context person azure function)
* Exception - eventualitypoc-dev-kv (dev keyvault) (24 character limit for key vault name)

## Built With

* [xAPI](https://xapi.com/)
* [Reactive extensions](http://reactivex.io/)
* [Azure Event grid](https://azure.microsoft.com/en-us/services/event-grid/)
* [Azure Cosmos DB](https://azure.microsoft.com/en-us/services/cosmos-db/)
* [Azure Functions](https://azure.microsoft.com/en-us/services/functions/)
* [SignalR](https://www.asp.net/signalr)
* [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/)

## Contributing

This is a proof-of-concept only and will not be developed further.

## Authors

* **Matthew de Nobrega** - [MatthewDeNobrega](https://github.com/matthewdenobrega)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
