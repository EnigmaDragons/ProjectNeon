using UnityEngine;

public class MouseTargetHoverCharacterV2 : OnMessage<CharacterHoverChanged, TargetSelectionBegun, TargetSelectionFinished>
{
    [SerializeField] private BattlePlayerTargetingStateV2 targetingState;
    
    private bool _isSelectingTargets;

    protected override void Execute(CharacterHoverChanged msg)
    {
        if (!_isSelectingTargets)
            return;
        if (msg.HoverCharacter.IsMissing)
            targetingState.StopIndicating();
        else
            targetingState.IndicateMember(msg.HoverCharacter.Value.Member.Id);
    }

    protected override void Execute(TargetSelectionBegun msg)
    {
        _isSelectingTargets = true;
    }

    protected override void Execute(TargetSelectionFinished msg)
    {
        _isSelectingTargets = false;
    }
}