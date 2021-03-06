using System;
using System.Linq;
using UnityEngine;

[Obsolete]
public class MouseTargetHoverCharacter : OnMessage<CharacterHoverChanged, TargetSelectionBegun, SelectionPossibleTargetsAvailable, TargetSelectionFinished>
{
    [SerializeField] private BattlePlayerTargetingState targeting;

    private int[] _possibleMembersIds = Array.Empty<int>();
    private bool _isSelectingTargets;
    private Maybe<HoverSpriteCharacter> _char = Maybe<HoverSpriteCharacter>.Missing();
    
    private void UpdateHover(Maybe<HoverSpriteCharacter> hovered)
    {
        if (!_isSelectingTargets)
            return;
        
        if (_char.IsPresent && hovered.IsMissing || _char.Value != hovered.Value)
            ResetLast();

        if (!hovered.IsPresent)
            return;

        var id = hovered.Value.Member.Id;
        if (_possibleMembersIds.Contains(id))
        {
            _char = hovered;
            _char.Value.SetAction(
                () => Message.Publish(new ConfirmTargetSelectionRequested()), 
                () => Message.Publish(new CancelTargetSelectionRequested()));
            targeting.MoveTo(id);
        }
    }
    
    private void ResetLast()
    {
        if (_char.IsPresent)
        {
            _char.Value.Revert();
            _char = Maybe<HoverSpriteCharacter>.Missing();
        }
    }

    protected override void Execute(CharacterHoverChanged msg)
    {
        msg.HoverCharacter.IfPresent(h => h.SetIsHovered());
    }

    protected override void Execute(TargetSelectionBegun msg)
    {
        _isSelectingTargets = true;
    }

    protected override void Execute(SelectionPossibleTargetsAvailable msg)
    {
        _possibleMembersIds = msg.Targets.SelectMany(t => t.Members.Select(m => m.Id)).Distinct().ToArray();
    }

    protected override void Execute(TargetSelectionFinished msg)
    {
        ResetLast();
        _isSelectingTargets = false;
        _possibleMembersIds = Array.Empty<int>();
    }
}
