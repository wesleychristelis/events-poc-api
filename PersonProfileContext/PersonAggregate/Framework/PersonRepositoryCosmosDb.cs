using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Framework
{
    public class PersonRepositoryCosmosDb : IPersonRepository
    {
        private const string _databaseName = "PersonContextDB";
        private const string _decisionCollectionName = "PersonDecision";
        private readonly Uri _decisionCollectionUri;
        private readonly DocumentClient _documentClient;
        private const string _perceptionCollectionName = "PersonPerception";
        private readonly Uri _perceptionCollectionUri;
        private readonly FeedOptions _queryOptions;

        #region Constructor
        public PersonRepositoryCosmosDb(DocumentClient documentClient)
        {
            _documentClient = documentClient ?? throw new ArgumentException("Tried to create a Person repository without a document client");
            _decisionCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _decisionCollectionName);
            _perceptionCollectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _perceptionCollectionName);
            _queryOptions = new FeedOptions { MaxItemCount = -1 };
        }
        #endregion

        #region Public
        public async Task InitializeAsync()
        {
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName });
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName),
                new DocumentCollection { Id = _decisionCollectionName });
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseName),
                new DocumentCollection { Id = _perceptionCollectionName });
        }

        public Person RetrievePerson(string id)
        {
            var query = new SqlQuerySpec(
                    "SELECT * FROM c WHERE c.object.id = @someId",
                    new SqlParameterCollection(new SqlParameter[] { new SqlParameter { Name = "@someId", Value = id } }));

            var lastJObject = _documentClient
                .CreateDocumentQuery<JObject>(_decisionCollectionUri, query, _queryOptions)
                .AsEnumerable()
                .LastOrDefault();

            if (lastJObject == null) return null;

            var lastStatement = new StatementExtension(lastJObject);

            return lastStatement?.targetData()?.ToObject<Person>();
        }

        public async Task SaveDecisionAsync(StatementExtension statement)
        {
            if (statement == null) return;

            statement.prepareToPersist();

            await _documentClient.CreateDocumentAsync(_decisionCollectionUri, statement.ToJObject());
        }

        public async Task SavePerceptionAsync(StatementExtension statement)
        {
            if (statement == null) return;

            statement.prepareToPersist();

            await _documentClient.CreateDocumentAsync(_perceptionCollectionUri, statement.ToJObject());
        }
        #endregion
    }
}