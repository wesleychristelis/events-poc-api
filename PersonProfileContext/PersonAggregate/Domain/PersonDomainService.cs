using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using System;

namespace PersonProfileContext.PersonAggregate.Domain
{
    public class PersonDomainService
    {
        #region Public
        public StatementExtension CreatePersonDecider(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verbString() != Verb.PersonCreationRequested) throw new InvalidOperationException("Incorrect verb to create person");

            if (person?.Name == null)
            {
                return perceptionStatement.createSuccessor(Verb.PersonCreationFailed, person);
            }
            else
            {
                person.PopulateId();
                return perceptionStatement.createSuccessor(Verb.PersonCreated, person, person.Id);
            }   
        }

        public StatementExtension RetrievePersonDecider(StatementExtension perceptionStatement, Person person)
        {
            if (perceptionStatement?.verbString() != Verb.PersonRequested) throw new InvalidOperationException("Incorrect verb to retrieve person");

            return person != null ?
                perceptionStatement.createSuccessor(Verb.PersonRetrieved, person) :
                perceptionStatement.createSuccessor(Verb.PersonRetrievalFailed, null);
        }

        public StatementExtension UpdatePersonDecider(StatementExtension perceptionStatement, Person personToUpdate)
        {
            if (perceptionStatement?.verbString() != Verb.PersonUpdateRequested) throw new InvalidOperationException("Incorrect verb to update person");           

            var updatedPerson = perceptionStatement.targetData<Person>();

            return personToUpdate != null && updatedPerson?.Name != null ?
                perceptionStatement.createSuccessor(Verb.PersonUpdated, updatedPerson) :
                perceptionStatement.createSuccessor(Verb.PersonUpdateFailed, null);
        }
        #endregion
    }
}
