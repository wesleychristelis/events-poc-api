using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using TinCan;

namespace EventualityPOCApi.Shared.Xapi
{
    public class StatementExtension : Statement
    {
        public const string ActivityDefinitionDataExtension = "http://eventuality.poc/xapi/object/extension/data";
        public const string ClientIdExtension = "http://eventuality.poc/xapi/context/extension/client-id-extension";
        public const string ClientPrecursorIdExtension = "http://eventuality.poc/xapi/context/extension/client-precursor-id-extension";

        #region Constructor
        public StatementExtension() : base() {}

        public StatementExtension(JObject jObject) : base(jObject) {}
        #endregion

        #region Public
        public StatementExtension createSuccessor(Uri verbId)
        {
            return new StatementExtension()
            {
                actor = actor != null ? new Agent(actor.ToJObject()) : null,
                authority = authority != null ? new Agent(authority.ToJObject()) : null,
                context = createSuccessorContext(),
                id = Guid.NewGuid(),
                target = target != null ? new Activity(target.ToJObject(TCAPIVersion.V103)) : null,
                timestamp = DateTime.Now,
                verb = new TinCan.Verb()
                {
                    id = verbId
                },
                version = TCAPIVersion.V103
            };
        }

        public StatementExtension createSuccessor(Uri verbId, object targetData, string targetId = null)
        {
            var successorStatement = createSuccessor(verbId);

            successorStatement.populateTarget(targetData, targetId);

            return successorStatement;
        }

        public StatementExtension createSuccessor(string verbId, object targetData, string targetId = null)
        {
            return createSuccessor(new Uri(verbId), targetData, targetId);
        }

        public void prepareToPersist()
        {
            id = id ?? Guid.NewGuid();
            stored = DateTime.Now;
        }

        public void populateTarget(object targetData, string targetId = null)
        {
            var activity = (Activity)target ?? new Activity();

            if (targetId != null)
            {
                activity.id = targetId;
            }

            if (targetData != null)
            {
                var targetDataJObject = convertTargetToJobject(targetData);

                activity.definition = activity.definition ?? new ActivityDefinition();
                var extensionsJObject = new JObject
            {
                { $"{ActivityDefinitionDataExtension}", targetDataJObject }
            };
                activity.definition.extensions = new TinCan.Extensions(extensionsJObject);
            }

            target = activity;
        }

        public JObject targetData()
        {
            return (JObject)((Activity)target)?.definition?.extensions?.ToJObject()
                ?.GetValue(ActivityDefinitionDataExtension);
        }

        public T targetData<T>()
        {
            var targetJObject = targetData();

            return targetJObject != null ? targetJObject.ToObject<T>() : default(T);
        }

        public string targetId()
        {
            return ((Activity)target)?.id;
        }

        public string verbString()
        {
            return verb?.id?.ToString();
        }
        #endregion

        #region Private
        private Context createSuccessorContext()
        {
            if (context == null && id == null) return null;

            var successorContext = context != null ? new Context(context.ToJObject()) : new Context();

            if (id != null) successorContext.statement = new StatementRef(id.Value);

            if (context?.extensions != null)
            {
                var extensionJObject = context.extensions.ToJObject();
                extensionJObject.Remove(ClientPrecursorIdExtension);

                var clientId = extensionJObject.GetValue(ClientIdExtension);
                if (clientId != null)
                {
                    extensionJObject.Add(ClientPrecursorIdExtension, clientId);
                    extensionJObject.Remove(ClientIdExtension);
                }

                successorContext.extensions = new TinCan.Extensions(extensionJObject);
            }

            return successorContext;
        }

        private JObject convertTargetToJobject(object targetData)
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JObject.FromObject(targetData, serializer);
        }
        #endregion
    }
}
