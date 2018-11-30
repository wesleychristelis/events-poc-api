using EventualityPOCApi.Shared.Xapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace EventualityPOCApi.SharedTest.Xapi.StatementExtensionTest
{
    [TestClass]
    public class PrepareToPersistTest
    {
        [TestMethod]
        public void Prepare_to_persist_should_set_an_id_if_one_is_not_set_already()
        {
            var statement = new StatementExtension();

            statement.prepareToPersist();

            Assert.IsNotNull(statement.id);
        }

        [TestMethod]
        public void Prepare_to_persist_should_not_overwrite_an_existing_id()
        {
            var existingId = Guid.Parse("9245fe4a-d402-451c-b9ed-9c1a04247482");
            var statement = new StatementExtension()
            {
                id = existingId
            };

            statement.prepareToPersist();

            Assert.AreEqual(existingId, statement.id);
        }

        [TestMethod]
        public void Prepare_to_persist_should_set_the_stored_value()
        {
            var statement = new StatementExtension();

            statement.prepareToPersist();

            Assert.IsNotNull(statement.stored);
        }

        [TestMethod]
        public void Prepare_to_persist_should_overwrite_an_existing_stored_value()
        {
            var existingStored = DateTime.Parse("2008-11-01T19:35:00.0000000Z");
            var statement = new StatementExtension();

            statement.prepareToPersist();

            Assert.AreNotEqual(existingStored, statement.stored);
        }
    }
}
