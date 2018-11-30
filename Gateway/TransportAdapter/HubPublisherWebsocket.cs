using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Shared.Framework;
using Microsoft.AspNetCore.SignalR;

namespace EventualityPOCApi.Gateway.TransportAdapter
{
    public class HubPublisherWebsocket
    {
        private IDecisionChannel _decisionChannel;
        private IHubContext<HubSubscriberWebsocket> _hubSubContext;

        #region Constructor
        public HubPublisherWebsocket(IHubContext<HubSubscriberWebsocket> hubContext, IDecisionChannel decisionChannel)
        {
            _hubSubContext = hubContext;
            _decisionChannel = decisionChannel;
        }
        #endregion

        #region Public
        public void RegisterOutgoingHandler()
        {
            _decisionChannel.RegisterHandlerAsync(async (sw) =>
            {
                await _hubSubContext.Clients.Client(sw.Subject).SendAsync("Decision", sw.Data.ToJObject());
            });
        }
        #endregion
    }
}
