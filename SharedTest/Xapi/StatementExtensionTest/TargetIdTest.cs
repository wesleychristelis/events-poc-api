using Microsoft.VisualStudio.TestTools.UnitTesting;
using EventualityPOCApi.Shared.Xapi;
using TinCan;

namespace SharedTest.Xapi.StatementExtensionTest
{
    [TestClass]
    public class TargetIdTest
    {
        [TestMethod]
        public void Target_id_should_handle_empty_target()
        {
            var statementExtension = new StatementExtension();

            Assert.IsNull(statementExtension.targetId());
        }

        [TestMethod]
        public void Target_id_should_handle_empty_target_id()
        {
            var statementExtension = new StatementExtension()
            {
                target = new Activity()
            };

            Assert.IsNull(statementExtension.targetId());
        }

        [TestMethod]
        public void Target_id_should_return_target_id_if_it_is_set()
        {
            var statementExtension = new StatementExtension()
            {
                target = new Activity()
                {
                    id = "http://eventualityPOC.com/test/1"
                }
            };

            Assert.AreEqual("http://eventualityPOC.com/test/1", statementExtension.targetId());
        }
    }
}
