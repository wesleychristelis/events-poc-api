using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventualityPOCApi.Shared.Xapi;
using System;
using TinCan;
using Newtonsoft.Json.Linq;

namespace EventualityPOCApi.SharedTest.Xapi.StatementExtensionTest
{
    [TestClass]
    public class CreateSuccessorTest
    {
        private readonly Uri VerbId = new Uri(Shared.Xapi.Verb.SystemTest);

        [TestMethod]
        public void Create_successor_should_not_return_the_precursor()
        {
            var precursorStatement = new StatementExtension();

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreNotEqual(precursorStatement, successorStatement);
        }

        [TestMethod]
        public void Create_successor_should_not_modify_the_precursor()
        {
            var precursorStatement = new StatementExtension()
            {
                actor = new Agent()
                {
                    mbox = "test@test.com"
                }
            };

            var precursorString = precursorStatement.ToString();

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreEqual(precursorString, precursorStatement.ToString());
        }

        [TestMethod]
        public void Create_successor_should_clone_and_copy_actor()
        {
            var precursorStatement = new StatementExtension()
            {
                actor = new Agent()
                {
                    mbox = "test@test.com"
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreNotEqual(precursorStatement.actor, successorStatement.actor);
            Assert.AreEqual(precursorStatement.actor.mbox, successorStatement.actor.mbox);
        }

        [TestMethod]
        public void Create_successor_should_clone_and_copy_authority()
        {
            var precursorStatement = new StatementExtension()
            {
                authority = new Agent()
                {
                    name = "External LMS"
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreNotEqual(precursorStatement.authority, successorStatement.authority);
            Assert.AreEqual(precursorStatement.authority.name, successorStatement.authority.name);
        }

        [TestMethod]
        public void Create_successor_should_not_populate_the_context_if_there_is_no_precursor_context_and_no_precursor_id()
        {
            var precursorStatement = new StatementExtension();

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.IsNull(successorStatement.context);
        }

        [TestMethod]
        public void Create_successor_should_create_a_context_if_there_is_a_precursor_id()
        {
            var precursorId = Guid.Parse("9245fe4a-d402-451c-b9ed-9c1a04247483");
            var precursorStatement = new StatementExtension()
            {
                id = precursorId
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.IsNotNull(successorStatement.context);
            Assert.AreEqual(successorStatement.context.statement.id, precursorId);
        }

        [TestMethod]
        public void Create_successor_should_populate_the_precursor_id_on_the_successor()
        {
            var precursorPrecursorId = Guid.Parse("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var precursorId = Guid.Parse("9245fe4a-d402-451c-b9ed-9c1a04247483");
            var precursorStatement = new StatementExtension() {
                context = new Context()
                {
                    statement = new StatementRef()
                    {
                        id = precursorPrecursorId
                    }
                },
                id = precursorId
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreEqual(precursorId, successorStatement.context.statement.id);
        }

        [TestMethod]
        public void Create_successor_should_copy_across_other_context_properties()
        {
            var language = "es";
            var platform = "http://eventualityPOC.com";
            var precursorStatement = new StatementExtension()
            {
                context = new Context()
                {
                    language = language,
                    platform = platform
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreEqual(language, successorStatement.context.language);
            Assert.AreEqual(platform, successorStatement.context.platform);
        }

        [TestMethod]
        public void Create_successor_should_update_the_client_precursor_extension_properties()
        {
            var clientId = "1234";
            var precursorClientExtension = new JObject
            {
                { StatementExtension.ClientIdExtension, clientId }
            };
            var precursorStatement = new StatementExtension()
            {
                context = new Context()
                {
                    extensions = new TinCan.Extensions(precursorClientExtension)
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.IsNull(successorStatement.context.extensions.ToJObject().GetValue(StatementExtension.ClientIdExtension));
            Assert.AreEqual(clientId, successorStatement.context.extensions.ToJObject().GetValue(StatementExtension.ClientPrecursorIdExtension));
        }

        [TestMethod]
        public void Create_successor_should_copy_across_other_precursor_extension_properties()
        {
            var testExtensionKey = "http://eventuality.poc/xapi/context/extension/test";
            var testExtensionValue = "testValue";
            var precursorExtension = new JObject
            {
                { testExtensionKey, testExtensionValue }
            };
            var precursorStatement = new StatementExtension()
            {
                context = new Context()
                {
                    extensions = new TinCan.Extensions(precursorExtension)
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreEqual(testExtensionValue, successorStatement.context.extensions.ToJObject().GetValue(testExtensionKey));
        }

        [TestMethod]
        public void Create_successor_should_create_a_new_id_for_the_successor()
        {
            var precursorStatement = new StatementExtension() {};

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreNotEqual(precursorStatement.id, successorStatement.id);
            Assert.IsNotNull(successorStatement.id);
        }

        [TestMethod]
        public void Create_successor_should_clone_and_copy_target()
        {
            var precursorStatement = new StatementExtension()
            {
                target = new Activity()
                {
                    id = "http://eventualityPOC.com/test/1"
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId);

            Assert.AreNotEqual(precursorStatement.target, successorStatement.target);
            Assert.IsNotNull(precursorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id"));
            Assert.IsNotNull(successorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id"));
            Assert.AreEqual(precursorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id").ToString(), 
                successorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id").ToString());
        }

        [TestMethod]
        public void Create_successor_should_populate_the_target_id_if_it_is_supplied()
        {
            var newId = "http://eventualityPOC.com/test/2";

            var precursorStatement = new StatementExtension();

            var successorStatement = precursorStatement.createSuccessor(VerbId, null, newId);

            Assert.IsNotNull(successorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id"));
            Assert.AreEqual(successorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id").ToString(), newId);
        }

        [TestMethod]
        public void Create_successor_should_overwrite_the_target_id_if_a_new_target_id_is_supplied()
        {
            var oldId = "http://eventualityPOC.com/test";
            var newId = "http://eventualityPOC.com/test/2";

            var precursorStatement = new StatementExtension()
            {
                target = new Activity()
                {
                    id = oldId
                }
            };

            var successorStatement = precursorStatement.createSuccessor(VerbId, null, newId);

            Assert.IsNotNull(successorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id"));
            Assert.AreEqual(successorStatement.target.ToJObject(TCAPIVersion.V103).GetValue("id").ToString(), newId);
        }
    }
}
