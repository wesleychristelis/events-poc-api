using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Shared.Framework;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.Component.PersonProfileContext
{
    public class PersonComponent
    {
        private readonly IDecisionChannel _decisionChannel;
        private readonly ILogger<PersonComponent> _logger;
        private readonly IPerceptionChannel _perceptionChannel;
        private readonly IPersonRepository _personRepository;

        #region Constructor
        public PersonComponent(IDecisionChannel decisionChannel, ILogger<PersonComponent> logger,
            IPerceptionChannel perceptionChannel, IPersonRepository personRepository)
        {
            _decisionChannel = decisionChannel;
            _logger = logger;
            _perceptionChannel = perceptionChannel;
            _personRepository = personRepository;
        }
        #endregion

        #region Public
        public void Configure()
        {
            // Note - the subscription should be set up to filter appropriately - can put extra checks here if needed
            _perceptionChannel.Observable().Subscribe(async sw => await HandleStatement(sw));
        }
        #endregion

        #region private
        private async Task HandleStatement(StatementWrapper statementWrapper)
        {
            try
            {
                var decisionStatement = await new PersonApplicationService().MakeDecisionAsync(statementWrapper.Data, _personRepository);
                var decisionStatementWrapper = new StatementWrapper(statementWrapper.Subject, decisionStatement);
                _decisionChannel.Next(decisionStatementWrapper);
            }
            catch (Exception exception)
            {
                _logger.LogError("Exception handling statement for person aggregate: " + exception.Message);
                _logger.LogError(exception.StackTrace);
            }
        }
        #endregion
    }
}
