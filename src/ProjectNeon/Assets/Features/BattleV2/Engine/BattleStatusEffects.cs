using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleStatusEffects : OnMessage<StatusEffectResolved>
{
    [SerializeField] private BattleState state;

    private readonly Queue<Member> _membersToProcess = new Queue<Member>();
    private bool _isProcessingStartOfTurn;
    
    public void ProcessStartOfTurnEffects()
    {
        _isProcessingStartOfTurn = true;
        foreach (var member in state.Members.Values) 
            _membersToProcess.Enqueue(member);
        ResolveNext();
    }
    
    public void ProcessEndOfTurnEffects()
    {
        _isProcessingStartOfTurn = false;
        foreach (var member in state.Members.Values) 
            _membersToProcess.Enqueue(member);
        ResolveNext();
    }

    private void ResolveNext()
    {
        Debug.Log("Resolve Next");
        if (_membersToProcess.Any())
        {        
            Debug.Log("Resolve Next - More Members");
            Message.Subscribe<SequenceFinished>(_ =>
            {
                Message.Unsubscribe(this);
                Message.Subscribe<StatusEffectResolved>(Execute, this);
                Message.Publish(new StatusEffectResolved());;
            }, this);
            
            var member = _membersToProcess.Dequeue().State;
            var effectPayloadProvider = _isProcessingStartOfTurn
                ? member.GetTurnStartEffects()
                : member.GetTurnEndEffects();
            SequenceMessage.Queue(effectPayloadProvider);
        }
        else
        {
            Debug.Log("Resolve Next - Finished All Members");
            if (_isProcessingStartOfTurn)
                Message.Publish(new StartOfTurnEffectsStatusResolved());
            else
                Message.Publish(new EndOfTurnStatusEffectsResolved());
        }
    }

    protected override void Execute(StatusEffectResolved msg)
    {
        Debug.Log("Received Status Effect Resolved");
        ResolveNext();
    }
}
