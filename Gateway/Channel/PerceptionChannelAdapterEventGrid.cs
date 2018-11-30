using EventualityPOCApi.Gateway.Configuration;
using EventualityPOCApi.Shared.Framework;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;

namespace EventualityPOCApi.Gateway.Channel
{
    public class PerceptionChannelAdapterEventGrid
    {
        private readonly EventGridClient _eventGridClient;
        protected string _topicHostName;
        
        #region Constructor
        public PerceptionChannelAdapterEventGrid(EventGridClient eventGridClient, EventGridConfiguration eventGridConfiguration)
        {
            _eventGridClient = eventGridClient;
            _topicHostName = new Uri(eventGridConfiguration.PersonProfileContextPerceptionTopicUrl).Host;
        }
        #endregion

        #region Public
        public void Configure(IChannel channel)
        {
            channel.RegisterHandlerAsync((StatementWrapper statementWrapper) =>
            {
                return _eventGridClient.PublishEventsAsync(_topicHostName, CreateEventGridEventList(statementWrapper));
            });
        }
        #endregion

        #region private
        private List<EventGridEvent> CreateEventGridEventList(StatementWrapper statementWrapper)
        {
            if (statementWrapper == null) return new List<EventGridEvent>();

            return new List<EventGridEvent>
                {
                    new EventGridEvent()
                    {
                        Data = statementWrapper.Data.ToJObject(),
                        DataVersion = statementWrapper.DataVersion,
                        EventTime = statementWrapper.EventTime,
                        EventType = statementWrapper.EventType,
                        Id = statementWrapper.Id,
                        Subject = statementWrapper.Subject,
                    }
                };
        }
        #endregion
    }
}
