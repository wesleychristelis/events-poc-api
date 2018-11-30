using EventualityPOCApi.Gateway.Channel;
using EventualityPOCApi.Shared.Framework;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace EventualityPOCApi.Channel
{
    public class ChannelRx
    {
        private readonly Subject<StatementWrapper> _subject;

        #region Constructor
        public ChannelRx()
        {
            _subject = new Subject<StatementWrapper>();
        }
        #endregion

        #region Public
        public IObservable<StatementWrapper> Observable()
        {
            return _subject;
        }

        public void Next(StatementWrapper statementWrapper)
        {
            _subject.OnNext(statementWrapper);
        }

        public void RegisterHandler(Action<StatementWrapper> handler)
        {
            _subject.Subscribe(handler);
        }

        public void RegisterHandlerAsync(Func<StatementWrapper, Task> handler)
        {
            _subject.Subscribe(async (sw) => await handler(sw));
        }
        #endregion
    }

    public class DecisionChannelRx : ChannelRx, IDecisionChannel
    {
        #region Constructor
        public DecisionChannelRx() : base() { }
        #endregion
    }

    public class PerceptionChannelRx : ChannelRx, IPerceptionChannel
    {
        #region Constructor
        public PerceptionChannelRx() : base() { }
        #endregion
    }
}
