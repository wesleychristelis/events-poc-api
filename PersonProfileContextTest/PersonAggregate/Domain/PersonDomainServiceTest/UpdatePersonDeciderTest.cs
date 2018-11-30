using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonProfileContext.PersonAggregate.Domain;
using System;

namespace EventualityPOCApi.PersonProfileContextTest.PersonAggregate.Domain.PersonDomainServiceTest
{
    [TestClass]
    public class UpdatePersonDeciderTest
    {
        private PersonDomainService _personDomainService = new PersonDomainService();
        
        [TestMethod]
        public void Update_person_decider_should_throw_if_the_perception_statement_does_not_have_a_verb()
        {
            var perceptionStatement = new StatementExtension();

            Assert.ThrowsException<InvalidOperationException>(() => _personDomainService.UpdatePersonDecider(perceptionStatement, null));
        }

        [TestMethod]
        public void Update_person_decider_should_throw_if_the_perception_statement_verb_is_incorrect()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonRequested)
                }
            };

            Assert.ThrowsException<InvalidOperationException>(() => _personDomainService.UpdatePersonDecider(perceptionStatement, null));
        }

        [TestMethod]
        public void Update_person_decider_should_fail_if_no_person_is_submitted()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonUpdateRequested)
                }
            };

            var decisionStatement = _personDomainService.UpdatePersonDecider(perceptionStatement, null);

            Assert.AreEqual(decisionStatement.verb.id, Verb.PersonUpdateFailed);
        }

        [TestMethod]
        public void Update_person_decider_should_fail_if_the_person_does_not_have_a_name()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonUpdateRequested)
                }
            };
            perceptionStatement.populateTarget(new Person());

            var decisionStatement = _personDomainService.UpdatePersonDecider(perceptionStatement, null);

            Assert.AreEqual(decisionStatement.verb.id, Verb.PersonUpdateFailed);
        }

        [TestMethod]
        public void Update_person_decider_should_update_the_person_if_the_person_has_a_name()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonUpdateRequested)
                }
            };
            var person = new Person()
            {
                Name = "Test"
            };
            perceptionStatement.populateTarget(person);

            var decisionStatement = _personDomainService.UpdatePersonDecider(perceptionStatement, person);

            Assert.AreEqual(decisionStatement.verb.id, Verb.PersonUpdated);
            Assert.AreEqual(decisionStatement.targetData<Person>().Name, person.Name);
        }
    }
}
