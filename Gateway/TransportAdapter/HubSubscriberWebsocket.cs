using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Shared.Framework;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace EventualityPOCApi.Gateway.TransportAdapter
{
    public class HubSubscriberWebsocket : Hub
    {
        private readonly IPerceptionChannel _perceptionChannel;

        #region Constructor
        public HubSubscriberWebsocket(IPerceptionChannel perceptionChannel)
        {
            _perceptionChannel = perceptionChannel;
        }
        #endregion

        #region Public
        public void Perception(JObject payload)
        {
            var statementWrapper = new StatementWrapper(this.Context.ConnectionId, new Shared.Xapi.StatementExtension(payload));
            _perceptionChannel.Next(statementWrapper);
        }
        #endregion
    }
}
