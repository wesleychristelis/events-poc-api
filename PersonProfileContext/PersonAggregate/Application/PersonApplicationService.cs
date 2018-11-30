using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using PersonProfileContext.PersonAggregate.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application
{
    public class PersonApplicationService
    {
        #region Public
        public Task<StatementExtension> MakeDecisionAsync(StatementExtension perceptionStatement, IPersonRepository personRepository)
        {
            return MakeDecisionAsync(perceptionStatement, personRepository, new PersonDomainService());
        }

        public async Task<StatementExtension> MakeDecisionAsync(StatementExtension perceptionStatement, IPersonRepository personRepository,
            PersonDomainService personDomainService)
        {
            if (perceptionStatement == null) return null;

            if (personRepository == null) throw new ArgumentException("Person application service asked to make a decision without a person repository");
            if (personDomainService == null) throw new ArgumentException("Person application service asked to make a decision without a person domain service");

            var personVerbs = new List<string>() { Verb.PersonCreationRequested, Verb.PersonRequested, Verb.PersonUpdateRequested };
            if (!personVerbs.Contains(perceptionStatement.verbString())) return null;

            StatementExtension decisionStatement = null;

            await personRepository.SavePerceptionAsync(perceptionStatement);

            switch (perceptionStatement?.verbString())
            {
                case Verb.PersonCreationRequested:
                    var personToCreate = perceptionStatement.targetData<Person>();
                    decisionStatement = personDomainService.CreatePersonDecider(perceptionStatement, personToCreate);
                    break;
                case Verb.PersonRequested:
                    var idOfPersonRequested = perceptionStatement.targetId();
                    var personRequested = personRepository.RetrievePerson(idOfPersonRequested);
                    decisionStatement = personDomainService.RetrievePersonDecider(perceptionStatement, personRequested);
                    break;
                case Verb.PersonUpdateRequested:
                    var idOfPerson = perceptionStatement.targetId();
                    var personToUpdate = personRepository.RetrievePerson(idOfPerson);
                    decisionStatement = personDomainService.UpdatePersonDecider(perceptionStatement, personToUpdate);
                    break;
            }

            await personRepository.SaveDecisionAsync(decisionStatement);

            return decisionStatement;
        }
        #endregion
    }
}