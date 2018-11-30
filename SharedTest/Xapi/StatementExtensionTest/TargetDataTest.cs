using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using EventualityPOCApi.Shared.Xapi;
using TinCan;

namespace EventualityPOCApi.SharedTest.Xapi.StatementExtensionTest
{
    [TestClass]
    public class TargetDataTest
    {
        [TestMethod]
        public void Target_data_should_handle_empty_target()
        {
            var statementExtension = new StatementExtension();

            Assert.IsNull(statementExtension.targetData());
        }

        [TestMethod]
        public void Target_data_generic_should_handle_empty_target()
        {
            var statementExtension = new StatementExtension();

            Assert.IsNull(statementExtension.targetData<Person>());
        }

        [TestMethod]
        public void Target_data_should_handle_empty_target_definition()
        {
            var statementExtension = new StatementExtension()
            {
                target = new Activity()
            };

            Assert.IsNull(statementExtension.targetData());
        }

        [TestMethod]
        public void Target_data_generic_should_handle_empty_target_definition()
        {
            var statementExtension = new StatementExtension()
            {
                target = new Activity()
            };

            Assert.IsNull(statementExtension.targetData<Person>());
        }

        [TestMethod]
        public void Target_data_should_handle_empty_target_definition_extensions()
        {
            var statementExtension = new StatementExtension()
            {
                target = new Activity()
                {
                    definition = new ActivityDefinition()
                }
            };

            Assert.IsNull(statementExtension.targetData());
        }

        [TestMethod]
        public void Target_data_generic_should_handle_empty_target_definition_extensions()
        {
            var statementExtension = new StatementExtension()
            {
                target = new Activity()
                {
                    definition = new ActivityDefinition()
                }
            };

            Assert.IsNull(statementExtension.targetData<Person>());
        }

        [TestMethod]
        public void Target_data_should_clone_and_return_the_data_if_it_is_set()
        {
            var targetData = JObject.FromObject(new Person() { Name = "Test" });
            var extensionsJObject = new JObject()
            {
                { $"{StatementExtension.ActivityDefinitionDataExtension}", targetData }
            };

            var statementExtension = new StatementExtension()
            {
                target = new Activity()
                {
                    definition = new ActivityDefinition()
                    {
                        extensions = new TinCan.Extensions(extensionsJObject)
                    }
                }
            };

            Assert.AreNotEqual(targetData, statementExtension.targetData());
            Assert.AreEqual(targetData.GetValue("Name").ToString(), statementExtension.targetData().GetValue("Name").ToString());
        }

        [TestMethod]
        public void Target_data_generic_should_return_the_data_if_it_is_set()
        {
            var targetData = new Person() { Name = "Test" };
            var targetDataJObject = JObject.FromObject(targetData);
            var extensionsJObject = new JObject()
            {
                { $"{StatementExtension.ActivityDefinitionDataExtension}", targetDataJObject }
            };

            var statementExtension = new StatementExtension()
            {
                target = new Activity()
                {
                    definition = new ActivityDefinition()
                    {
                        extensions = new TinCan.Extensions(extensionsJObject)
                    }
                }
            };

            Assert.AreNotEqual(targetData, statementExtension.targetData<Person>());
            Assert.AreEqual(targetData.Name, statementExtension.targetData<Person>().Name);
        }
    }
}
