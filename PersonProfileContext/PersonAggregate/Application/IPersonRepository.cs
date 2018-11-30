using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using System.Threading.Tasks;

namespace EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application
{
    public interface IPersonRepository
    {
        Task InitializeAsync();
        Person RetrievePerson(string id);
        Task SaveDecisionAsync(StatementExtension statement);
        Task SavePerceptionAsync(StatementExtension statement);
    }
}