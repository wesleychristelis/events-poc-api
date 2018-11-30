using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Framework;
using EventualityPOCApi.Shared.Framework;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventualityPOCApi.Cloud.PersonProfileCloud
{
    public static class PersonComponent
    {
        private static readonly string _cosmosDBAccountEndpoint = Environment.GetEnvironmentVariable("CosmosDBAccountEndpoint");
        private static readonly string _cosmosDBAccountKey = Environment.GetEnvironmentVariable("CosmosDBAccountKey");
        private static readonly DocumentClient _documentClient = new DocumentClient(new Uri(_cosmosDBAccountEndpoint), _cosmosDBAccountKey);
        private static readonly EventGridClient _eventGridClient = new EventGridClient(new TopicCredentials(Environment.GetEnvironmentVariable("EventGridDecisionTopicKey")));
        private static readonly string _eventGridTopicHostName = new Uri(Environment.GetEnvironmentVariable("EventGridDecisionTopicUrl")).Host;
        private static readonly IPersonRepository _personRepository = new PersonRepositoryCosmosDb(_documentClient);

        [FunctionName("PersonComponent")]
        public static async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger logger)
        {
            try
            {
                var perceptionStatement = new StatementExtension((JObject)eventGridEvent.Data);

                StatementExtension decisionStatement = await new PersonApplicationService().MakeDecisionAsync(perceptionStatement, 
                    _personRepository);

                var decisionEvents = CreateEventGridEventList(new StatementWrapper(eventGridEvent.Subject, decisionStatement));

                await _eventGridClient.PublishEventsAsync(_eventGridTopicHostName, decisionEvents);
            }
            catch (Exception exception)
            {
                logger.LogError("Exception handling statement for person aggregate: " + exception.Message);
                logger.LogError(exception.StackTrace);
            }
        }

        private static List<EventGridEvent> CreateEventGridEventList(StatementWrapper statementWrapper)
        {
            if (statementWrapper == null) return new List<EventGridEvent>();

            return new List<EventGridEvent>
                {
                    new EventGridEvent()
                    {
                        Data = statementWrapper.Data.ToJObject(),
                        DataVersion = statementWrapper.DataVersion,
                        EventTime = statementWrapper.EventTime,
                        EventType = statementWrapper.EventType,
                        Id = statementWrapper.Id,
                        Subject = statementWrapper.Subject,
                    }
                };
        }
    }
}
