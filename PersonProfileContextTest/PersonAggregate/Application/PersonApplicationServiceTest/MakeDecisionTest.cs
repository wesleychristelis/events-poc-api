using EventualityPOCApi.Context.PersonProfileContext.PersonAggregate.Application;
using EventualityPOCApi.Shared.Xapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PersonProfileContext.PersonAggregate.Domain;
using System;
using System.Threading.Tasks;

namespace EventualityPOCApi.PersonProfileContextTest.PersonAggregate.Application.PersonApplicationServiceTest
{
    [TestClass]
    public class MakeDecisionTest
    {
        private PersonApplicationService _personApplicationService;
        private Mock<PersonDomainService> _personDomainService;
        private Mock<IPersonRepository> _personRepository;

        [TestInitialize()]
        public void Initialize()
        {
            _personApplicationService = new PersonApplicationService();
            _personDomainService = new Mock<PersonDomainService>();
            _personRepository = new Mock<IPersonRepository>();
        }

        [TestMethod]
        public void Make_Decision_async_should_do_nothing_if_statement_is_not_supplied()
        {
            var decisionStatement = _personApplicationService.MakeDecisionAsync(null, _personRepository.Object, 
                _personDomainService.Object).Result;

            Assert.IsNull(decisionStatement);
            _personRepository.Verify(pr => pr.SavePerceptionAsync(null), Times.Never);
            _personRepository.Verify(pr => pr.SaveDecisionAsync(decisionStatement), Times.Never);
        }

        [TestMethod]
        public void Make_Decision_async_should_do_nothing_if_statement_has_an_incorrect_verb()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.SystemTest)
                }
            };

            var decisionStatement = _personApplicationService.MakeDecisionAsync(perceptionStatement, _personRepository.Object,
                _personDomainService.Object).Result;

            Assert.IsNull(decisionStatement);
            _personRepository.Verify(pr => pr.SavePerceptionAsync(null), Times.Never);
            _personRepository.Verify(pr => pr.SaveDecisionAsync(decisionStatement), Times.Never);
        }

        [TestMethod]
        public void Make_Decision_async_should_throw_if_person_repository_is_not_supplied()
        {
            var perceptionStatement = new StatementExtension();

            Assert.ThrowsExceptionAsync<ArgumentException>(() =>  _personApplicationService.MakeDecisionAsync(perceptionStatement, null, 
                _personDomainService.Object));
        }

        [TestMethod]
        public void Make_Decision_async_should_throw_if_person_domain_service_is_not_supplied()
        {
            var perceptionStatement = new StatementExtension();

            Assert.ThrowsExceptionAsync<ArgumentException>(() => _personApplicationService.MakeDecisionAsync(perceptionStatement, 
                _personRepository.Object, null));
        }

        [TestMethod]
        public void Make_Decision_async_should_persist_the_perception_statement()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonRequested)
                }
            };

            var decisionStatement = _personApplicationService.MakeDecisionAsync(perceptionStatement, _personRepository.Object, 
                _personDomainService.Object);

            _personRepository.Verify(pr => pr.SavePerceptionAsync(perceptionStatement));
        }

        [TestMethod]
        public void Make_Decision_async_should_persist_the_decision_statement()
        {
            var perceptionStatement = new StatementExtension()
            {
                verb = new TinCan.Verb()
                {
                    id = new Uri(Verb.PersonRequested)
                }
            };

            var decisionStatement = _personApplicationService.MakeDecisionAsync(perceptionStatement, _personRepository.Object, 
                _personDomainService.Object).Result;

            _personRepository.Verify(pr => pr.SaveDecisionAsync(decisionStatement));
        }
    }
}
