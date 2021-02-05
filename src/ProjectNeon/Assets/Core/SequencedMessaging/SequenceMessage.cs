using System.Collections.Generic;

public static class SequenceMessage
{
    private static SequencedMessageQueue Msgs = new SequencedMessageQueue();
    public static void Queue(IPayloadProvider payloadProvider) => Msgs.Queue(payloadProvider);
    
    private sealed class SequencedMessageQueue
    {
        private int _seqId = 0;
        private int _effectIndex = 0;
        private readonly Queue<IPayloadProvider> _enqueuedPayloadQueues = new Queue<IPayloadProvider>();
        private IPayloadProvider _currentPayloadQueue = new NoPayload();

        public void Queue(IPayloadProvider queueData)
        {
            if (queueData.IsFinished())
                return;
            
            _enqueuedPayloadQueues.Enqueue(queueData);
            if (_currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count > 0)
                ProcessNext();
        }

        private void ProcessNext()
        {
            Message.Unsubscribe(this);
            if (_currentPayloadQueue.IsFinished())
            {
                Log.Info($"Sequence {_seqId} Finished");
                Message.Publish(new SequenceFinished());
                _seqId++;
                _effectIndex = -1;
            }
            
            while (_currentPayloadQueue.IsFinished() && _enqueuedPayloadQueues.Count > 0)
                _currentPayloadQueue = _enqueuedPayloadQueues.Dequeue();

            if (_currentPayloadQueue.IsFinished()) 
                return;

            _effectIndex++;
            var payloadData = _currentPayloadQueue.GetNext();
            var finishedTypeName = payloadData.Finished.IsGenericType
                ? payloadData.Finished.Name.Replace("1", payloadData.Finished.GenericTypeArguments[0].Name)
                : payloadData.Finished.Name;
            Log.Info($"Sequence {_seqId} - Effect {_effectIndex}. Type: {payloadData.Payload.GetType().Name}. Ending Type: {finishedTypeName}");
            Message.Subscribe(new MessageSubscription(payloadData.Finished, _ => ProcessNext(), this));
            Message.Publish(payloadData.Payload);
        }
    }
}
