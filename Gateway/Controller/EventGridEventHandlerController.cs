using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Shared.Framework;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace EventGridEventTrigger.DotNetCoreAPIApp.Controllers
{
    [Produces("application/json")]
    public class EventGridEventHandlerController : Controller
    {
        private readonly IDecisionChannel _decisionChannel;
        private readonly ILogger _logger;

        public EventGridEventHandlerController(IDecisionChannel decisionChannel, ILogger<EventGridEventHandlerController> logger)
        {
            _decisionChannel = decisionChannel;
            _logger = logger;
        }

        [HttpPost]
        [Route("api/EventGridEventHandler")]
        public IActionResult Post([FromBody] JArray requestJArray)
        {
            var requestContent = requestJArray.ToString();

            _logger.LogInformation($"Received events: {requestContent}");

            EventGridSubscriber eventGridSubscriber = new EventGridSubscriber();
            EventGridEvent[] eventGridEvents = eventGridSubscriber.DeserializeEventGridEvents(requestContent);

            foreach (EventGridEvent eventGridEvent in eventGridEvents)
            {
                if (eventGridEvent.Data is SubscriptionValidationEventData eventData)
                {
                    _logger.LogInformation($"Got SubscriptionValidation event data, validationCode: {eventData.ValidationCode},  validationUrl: {eventData.ValidationUrl}, topic: {eventGridEvent.Topic}");
                    // Do any additional validation (as required) such as validating that the Azure resource ID of the topic matches
                    // the expected topic and then return back the below response
                    var responseData = new SubscriptionValidationResponse()
                    {
                        ValidationResponse = eventData.ValidationCode
                    };

                    return Ok(responseData);
                }
                else
                {
                    var statement = new StatementExtension((JObject)eventGridEvent.Data);
                    var statementWrapper = new StatementWrapper(eventGridEvent.Subject, statement);
                    _decisionChannel.Next(statementWrapper);
                }
            }

            return Ok(null);
        }
    }
}