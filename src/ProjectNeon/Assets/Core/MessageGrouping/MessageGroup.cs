using System.Collections.Generic;

public static class MessageGroup
{
    private static MessageGroupQueue Msgs = new MessageGroupQueue();
    public static void Queue(IPayloadProvider payloadProvider) => Msgs.Queue(payloadProvider);
    
    private sealed class MessageGroupQueue
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
                Message.Publish(new MessageGroupFinished());
            else
            {
                var payloadData = _currentPayloadQueue.GetNext();
                Message.Subscribe(new MessageSubscription(payloadData.Finished, _ => ProcessNext(), this));
                Message.Publish(payloadData.Payload);
            }
        }
    }
}