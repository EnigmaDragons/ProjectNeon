using System.Collections.Generic;
using System;
using System.Linq;

public static class Message 
{
    private static readonly List<MessageSubscription> EventSubs = new List<MessageSubscription>();
    private static MessageQueue Msgs = new MessageQueue();

    public static int SubscriptionCount => Msgs.SubscriptionCount;
    public static void Publish(object payload) => Msgs.Enqueue(payload);
    public static void Subscribe<T>(Action<T> onEvent, object owner) => Subscribe(MessageSubscription.Create(onEvent, owner));

    public static MessageQueue SwapMessageQueues(MessageQueue newQueue)
    {
        var msgs = Msgs;
        Msgs = newQueue;
        return msgs;
    }

    private static void Subscribe(MessageSubscription subscription)
    {
        Msgs.Subscribe(subscription);
        EventSubs.Add(subscription);
    }

    public static void Unsubscribe(object owner)
    {
        Msgs.Unsubscribe(owner);
        EventSubs.Where(x => x.Owner.Equals(owner)).ForEach(x => EventSubs.Remove(x));
    }
    
    public sealed class MessageQueue
    {
        private readonly Dictionary<string, List<object>> _eventActions = new Dictionary<string, List<object>>();
        private readonly Dictionary<object, List<MessageSubscription>> _ownerSubscriptions = new Dictionary<object, List<MessageSubscription>>();

        private readonly Queue<object> _eventQueue = new Queue<object>();
        private bool _isPublishing;

        public int SubscriptionCount => _eventActions.Sum(e => e.Value.Count);

        public void Enqueue(object payload)
        {
            _eventQueue.Enqueue(payload);
            ProcessQueuedMessages();
        }

        public void Subscribe(MessageSubscription subscription)
        {
            var eventType = subscription.EventType.Name;
            if (!_eventActions.ContainsKey(eventType))
                _eventActions[eventType] = new List<object>();
            if (!_ownerSubscriptions.ContainsKey(subscription.Owner))
                _ownerSubscriptions[subscription.Owner] = new List<MessageSubscription>();
            _eventActions[eventType].Add(subscription.OnEvent);
            _ownerSubscriptions[subscription.Owner].Add(subscription);
        }

        public void Unsubscribe(object owner)
        {
            if (!_ownerSubscriptions.ContainsKey(owner))
                return;
            var events = _ownerSubscriptions[owner];
            for (var i = 0; i < _eventActions.Count; i++)
                _eventActions.ElementAt(i).Value.RemoveAll(x => events.Any(y => y.OnEvent.Equals(x)));
            _ownerSubscriptions.Remove(owner);
        }

        private void ProcessQueuedMessages()
        {
            if (_isPublishing) return;
            
            _isPublishing = true;
            while (_eventQueue.Any()) 
                Publish(_eventQueue.Dequeue());
            _isPublishing = false;
        }

        private void Publish(object payload)
        {
            var eventType = payload.GetType().Name;

            if (_eventActions.ContainsKey(eventType))
                foreach (var action in _eventActions[eventType].ToList())
                    ((Action<object>)action)(payload);
        }
    }
}
