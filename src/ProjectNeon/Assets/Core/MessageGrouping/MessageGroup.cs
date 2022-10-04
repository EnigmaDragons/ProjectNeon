﻿using System;
using System.Collections.Generic;

public static class MessageGroup
{
    private static MessageGroupQueue Msgs = new MessageGroupQueue();

    public static bool IsInProgress => !IsClear;
    public static bool IsClear => Msgs.IsClear;
    public static string CurrentName => Msgs.CurrentName;
    public static void Start(IPayloadProvider payloadProvider, Action onFinished) => Msgs.Start(payloadProvider, onFinished);
    public static void ExtendCurrentGroup(IPayloadProvider payloadProvider) => Msgs.ExtendCurrentGroup(payloadProvider);
    public static void TerminateAndClear() => Msgs.TerminateAndClear();

    private sealed class MessageGroupQueue
    {
        private Queue<IPayloadProvider> _futurePayloadProviderQueue = new Queue<IPayloadProvider>();
        private Queue<Action> _onFinishedQueue = new Queue<Action>();
        private Queue<IPayloadProvider> _payloadProviderQueue = new Queue<IPayloadProvider>();
        private Action _onFinished = () => { };
        private IPayloadProvider _payloadProvider = new NoPayload();
        public bool IsClear = true;
        
        public string CurrentName => _payloadProvider.Name;

        public void Start(IPayloadProvider payloadProvider, Action onFinished)
        {
            _futurePayloadProviderQueue.Enqueue(payloadProvider);
            _onFinishedQueue.Enqueue(onFinished);
            if (IsClear)
            {
                IsClear = false;
                ProcessNext();
            }
        }
        
        public void ExtendCurrentGroup(IPayloadProvider queueData)
        {
            if (!IsClear)
                _payloadProviderQueue.Enqueue(queueData);
            else
                Log.Warn("Can't extend a completed queue");
        }
        
        public void TerminateAndClear()
        {
            Message.Unsubscribe(this);
            _futurePayloadProviderQueue = new Queue<IPayloadProvider>();
            _onFinishedQueue = new Queue<Action>();
            _payloadProviderQueue = new Queue<IPayloadProvider>();
            _onFinished = () => { };
            _payloadProvider = new NoPayload();
            IsClear = true;
        }

        private void ProcessNext()
        {
            Message.Unsubscribe(this);
            while (_payloadProvider.IsFinished() && (_payloadProviderQueue.Count > 0 || _futurePayloadProviderQueue.Count > 0))
            {
                if (_payloadProviderQueue.Count > 0)
                    _payloadProvider = _payloadProviderQueue.Dequeue();
                else
                {
                    _onFinished();
                    _payloadProviderQueue.Enqueue(_futurePayloadProviderQueue.Dequeue());
                    _onFinished = _onFinishedQueue.Dequeue();
                }
            }
            if (_payloadProvider.IsFinished())
            {
                _onFinished();
                IsClear = true;
            }
            else
            {
                var payloadData = _payloadProvider.GetNext();
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