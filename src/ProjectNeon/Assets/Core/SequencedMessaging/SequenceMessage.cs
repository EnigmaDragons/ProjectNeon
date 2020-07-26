using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public static class SequenceMessage
{
    private static SequencedMessageQueue Msgs = new SequencedMessageQueue();
    public static void Queue(object payload) => Msgs.Queue(new SinglePayload(payload));
    public static void Queue(IPayloadProvider payloadProvider) => Msgs.Queue(payloadProvider);
    
    public sealed class SequencedMessageQueue
    {
        private readonly Queue<IPayloadProvider> _enqueuedPayloadQueues = new Queue<IPayloadProvider>();
        private IPayloadProvider _currentPayloadQueue = new NoPayload();

        public void Queue(IPayloadProvider queueData)
        {
            _enqueuedPayloadQueues.Enqueue(queueData);
            if (_currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count == 1)
                ProcessNext();
        }

        private void ProcessNext()
        {
            Message.Unsubscribe(this);
            while (_currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count > 0)
                _currentPayloadQueue = _enqueuedPayloadQueues.Dequeue();
            if (_currentPayloadQueue.IsFinished())
                Message.Publish(new SequenceFinished());
            else
            {
                var payloadData = _currentPayloadQueue.GetNext();
                Message.Subscribe(new MessageSubscription(payloadData.Finished, _ => ProcessNext(), this));
                Message.Publish(payloadData.Payload);
            }
        }
    }
}