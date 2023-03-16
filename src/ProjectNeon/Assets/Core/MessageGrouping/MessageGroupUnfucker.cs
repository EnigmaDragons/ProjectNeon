using System;
using UnityEngine;

public class MessageGroupUnfucker : MonoBehaviour
{
    [SerializeField] private float _secondsBeforeAssumedToBeFucked = 5;

    private float _t = 0;
    
    private void Update()
    {
        if (MessageGroup.IsClear)
            return;
        if (MessageGroup.IsDirty)
        {
            _t = _secondsBeforeAssumedToBeFucked;
            MessageGroup.IsDirty = false;
        }
        else
        {
            _t = Math.Max(0, _t - Math.Min(1, Time.deltaTime)); //dont let significant lag spikes risk causing an unfuck
            if (_t <= 0)
            {
                Log.Error($"Message Group got stuck on {MessageGroup.CurrentName}, terminating group and continuing");
                MessageGroup.TerminateAndClearCurrentPayloadProvider();      
            }
        }
    }
}