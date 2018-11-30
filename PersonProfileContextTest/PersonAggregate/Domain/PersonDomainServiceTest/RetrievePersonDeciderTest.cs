using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonProfileContext.PersonAggregate.Domain;
using System;

namespace EventualityPOCApi.PersonProfileContextTest.PersonAggregate.Domain.PersonDomainServiceTest
{
    [TestClass]
    public class RetrievePersonDeciderTest
    {
        private PersonDomainService _personDomainService = new PersonDomainService();
        
        [TestMethod]
        public void Retrieve_person_decider_should_throw_if_the_perception_statement_does_not_have_a_verb()
        {
            var perceptionStatement = new StatementExtension();

            Assert.ThrowsException<InvalidOperationException>(() => _personDomainService.RetrievePersonDecider(perceptionStatement, null));
        }

        [TestMethod]
        public void Retrieve_person_decider_should_throw_if_the_perception_statement_verb_is_incorrect()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonCreationRequested)
                }
            };

            Assert.ThrowsException<InvalidOperationException>(() => _personDomainService.RetrievePersonDecider(perceptionStatement, null));
        }

        [TestMethod]
        public void Retrieve_person_decider_should_fail_if_the_person_is_not_found()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonRequested)
                }
            };

            var responseStatement = _personDomainService.RetrievePersonDecider(perceptionStatement, null);

            Assert.AreEqual(responseStatement.verb.id, Verb.PersonRetrievalFailed);
        }

        [TestMethod]
        public void Retrieve_person_decider_should_return_the_person_if_the_person_is_found()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonRequested)
                }
            };
            var person = new Person()
            {
                Name = "Test"
            };

            var responseStatement = _personDomainService.RetrievePersonDecider(perceptionStatement, person);

            Assert.AreEqual(responseStatement.verb.id, Verb.PersonRetrieved);
            Assert.AreEqual(responseStatement.targetData<Person>().Name, person.Name);
        }
    }
}
