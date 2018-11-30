using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Framework;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EventualityPOCApi.Cloud.PersonProfileCloud
{
    public static class PersonCosmosDbCreator
    {
        private static readonly string _cosmosDBAccountEndpoint = Environment.GetEnvironmentVariable("CosmosDBAccountEndpoint");
        private static readonly string _cosmosDBAccountKey = Environment.GetEnvironmentVariable("CosmosDBAccountKey");
        private static readonly DocumentClient _documentClient = new DocumentClient(new Uri(_cosmosDBAccountEndpoint), _cosmosDBAccountKey);
        private static readonly IPersonRepository _personRepository = new PersonRepositoryCosmosDb(_documentClient);

        [FunctionName("PersonCosmosDbCreator")]
        public static async Task Run([TimerTrigger("0 30 2 * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            await _personRepository.InitializeAsync();
        }
    }
}
