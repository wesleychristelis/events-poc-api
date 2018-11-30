using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Domain;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PersonProfileContext.PersonAggregate.Domain;
using System;

namespace EventualityPOCApi.PersonProfileContextTest.PersonAggregate.Domain.PersonDomainServiceTest
{
    [TestClass]
    public class MakeDecisionTest
    {
        private PersonDomainService _personDomainService = new PersonDomainService();
        
        [TestMethod]
        public void Create_person_decider_should_throw_if_the_perception_statement_does_not_have_a_verb()
        {
            var perceptionStatement = new StatementExtension();

            Assert.ThrowsException<InvalidOperationException>(() => _personDomainService.CreatePersonDecider(perceptionStatement, null));
        }

        [TestMethod]
        public void Create_person_decider_should_throw_if_the_perception_statement_verb_is_incorrect()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonRequested)
                }
            };

            Assert.ThrowsException<InvalidOperationException>(() => _personDomainService.CreatePersonDecider(perceptionStatement, null));
        }

        [TestMethod]
        public void Create_person_decider_should_fail_if_no_person_is_submitted()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonCreationRequested)
                }
            };

            var decisionStatement = _personDomainService.CreatePersonDecider(perceptionStatement, null);

            Assert.AreEqual(decisionStatement.verb.id, Verb.PersonCreationFailed);
        }

        [TestMethod]
        public void Create_person_decider_should_fail_if_the_person_does_not_have_a_name()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonCreationRequested)
                }
            };
            perceptionStatement.populateTarget(new Person());

            var decisionStatement = _personDomainService.CreatePersonDecider(perceptionStatement, null);

            Assert.AreEqual(decisionStatement.verb.id, Verb.PersonCreationFailed);
        }

        [TestMethod]
        public void Create_person_decider_should_create_the_person_if_the_person_has_a_name()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonCreationRequested)
                }
            };
            var person = new Person()
            {
                Name = "Test"
            };
            perceptionStatement.populateTarget(person);

            var decisionStatement = _personDomainService.CreatePersonDecider(perceptionStatement, person);

            Assert.AreEqual(decisionStatement.verb.id, Verb.PersonCreated);
            Assert.AreEqual(decisionStatement.targetData<Person>().Name, person.Name);
        }

        [TestMethod]
        public void Create_person_decider_should_populate_the_new_target_id_after_creating_the_person()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonCreationRequested)
                }
            };
            var person = new Person()
            {
                Name = "Test"
            };
            perceptionStatement.populateTarget(person);

            var decisionStatement = _personDomainService.CreatePersonDecider(perceptionStatement, person);

            Assert.AreEqual(((TinCan.Activity)decisionStatement.target).id, decisionStatement.targetData<Person>().Id);
        }
    }
}
