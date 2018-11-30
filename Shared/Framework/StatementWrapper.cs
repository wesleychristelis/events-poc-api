using EventualityPOCApi.Shared.Xapi;
using System;

namespace EventualityPOCApi.Shared.Framework
{
    public class StatementWrapper
    {
        public StatementExtension Data { get; }
        public string DataVersion { get; }
        public DateTime EventTime { get; }
        public string EventType { get; }
        public string Id { get; }
        public string Subject { get; }

        #region Constructor
        public StatementWrapper(string connectionContextId, StatementExtension statement)
        {
            Data = statement ?? throw new ArgumentException("Tried to create an event wrapper without a statement");
            DataVersion = "1.0.0";
            EventTime = DateTime.Now;
            EventType = statement.verb.id.ToString();
            Id = Guid.NewGuid().ToString();
            Subject = connectionContextId;
        }
        #endregion
    }
}
