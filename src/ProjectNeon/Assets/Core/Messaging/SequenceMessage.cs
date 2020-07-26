using System;
using System.Collections.Generic;

public static class SequenceMessage
{
    private static SequencedMessageQueue Msgs = new SequencedMessageQueue();
    public static void Queue(object payload) => Msgs.Queue(payload);
    
    public sealed class SequencedMessageQueue
    {
        private readonly Queue<object> _enqueuedPayloads = new Queue<object>();
        private bool _processing = false;
    
        public void Queue(object payload)
        {
            _enqueuedPayloads.Enqueue(payload);
            if (!_processing)
                ProcessNext();
        }

        public void ProcessNext()
        {
            Message.Unsubscribe(this);
            _processing = _enqueuedPayloads.Count > 0;
            if (_processing)
            {
                var next = _enqueuedPayloads.Dequeue();
                var type = typeof(Finished<>).MakeGenericType(new Type[] { next.GetType() });
                Message.Subscribe(new MessageSubscription(type, _ => ProcessNext(), this));
                Message.Publish(next);
            }
            else
            {
                Message.Publish(new SequenceFinished());
            } 
        }
    }
}