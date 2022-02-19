using System;
using System.Collections.Generic;

public static class MessageGroup
{
    private static MessageGroupQueue Msgs = new MessageGroupQueue();

    public static bool IsInProgress => !IsClear;
    public static bool IsClear => Msgs.IsClear;
    public static string CurrentName => Msgs.CurrentName;
    public static void Start(IPayloadProvider payloadProvider, Action onFinished) => Msgs.Start(payloadProvider, onFinished);
    public static void Add(IPayloadProvider payloadProvider) => Msgs.Add(payloadProvider);
    public static void TerminateAndClear() => Msgs.TerminateAndClear();

    private sealed class MessageGroupQueue
    {
        private readonly Queue<IPayloadProvider> _enqueuedPayloadQueues = new Queue<IPayloadProvider>();
        private IPayloadProvider _currentPayloadQueue = new NoPayload();
        private Action _onFinished = () => { };

        public bool IsClear => _currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count == 0;
        public string CurrentName => _currentPayloadQueue.Name;

        public void Start(IPayloadProvider queueData, Action onFinished)
        {
            if (IsClear)
            {
                _onFinished = onFinished;
                _enqueuedPayloadQueues.Enqueue(queueData);
                ProcessNext();   
            }
            else
            {
                Log.Error($"Attempted to start a new MessageGroup while the Queue is currently processing - New Group: {queueData.Name} Active: {CurrentName}");
            }
        }
        
        public void Add(IPayloadProvider queueData)
        {
            _enqueuedPayloadQueues.Enqueue(queueData);
        }
        
        public void TerminateAndClear()
        {
            Message.Unsubscribe(this);
            _enqueuedPayloadQueues.Clear();
            _currentPayloadQueue = new NoPayload();
            _onFinished = () => { };
        }

        private void ProcessNext()
        {
            Message.Unsubscribe(this);
            while (_currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count > 0)
                _currentPayloadQueue = _enqueuedPayloadQueues.Dequeue();
            if (_currentPayloadQueue.IsFinished())
                _onFinished();
            else
            {
                var payloadData = _currentPayloadQueue.GetNext();
                Message.Subscribe(new MessageSubscription(payloadData.Finished, msg =>
                {
                    if (payloadData.FinishedCondition(msg))
                        ProcessNext();
                }, this));
                Message.Publish(payloadData.Payload);
            }
        }
    }
}