using System;
using System.Collections.Generic;

public static class MessageGroup
{
    private static MessageGroupQueue Msgs = new MessageGroupQueue();

    public static void Start(IPayloadProvider payloadProvider, Action onFinished) => Msgs.Start(payloadProvider, onFinished);
    public static void Add(IPayloadProvider payloadProvider) => Msgs.Add(payloadProvider);

    private sealed class MessageGroupQueue
    {
        private readonly Queue<IPayloadProvider> _enqueuedPayloadQueues = new Queue<IPayloadProvider>();
        private IPayloadProvider _currentPayloadQueue = new NoPayload();
        private Action _onFinished = () => { };

        public void Start(IPayloadProvider queueData, Action onFinished)
        {
            if (_currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count == 0)
            {
                _onFinished = onFinished;
                _enqueuedPayloadQueues.Enqueue(queueData);
                ProcessNext();   
            }
            else
            {
                Log.Error("Attempted to start a Queue in MessageGroup while a Queue is currently processing");
            }
        }
        
        public void Add(IPayloadProvider queueData)
        {
            _enqueuedPayloadQueues.Enqueue(queueData);
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