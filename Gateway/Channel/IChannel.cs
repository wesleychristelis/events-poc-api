using EventualityPOCApi.Shared.Framework;
using System;
using System.Threading.Tasks;

namespace EventualityPOCApi.Gateway.Channel
{
    /*
     * 
     */
    public interface IChannel
    {
        IObservable<StatementWrapper> Observable();
        void Next(StatementWrapper statementWrapper);
        void RegisterHandler(Action<StatementWrapper> handler);
        void RegisterHandlerAsync(Func<StatementWrapper, Task> handler);
    }

    public interface IDecisionChannel : IChannel { }
    public interface IPerceptionChannel : IChannel { }
}
