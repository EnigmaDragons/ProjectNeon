using UnityEngine;

public class EnemyHoverRightClickViewDetails : OnMessage<CharacterHoverChanged>
{
    [SerializeField] private BattleState state;

    private Maybe<HoverCharacter> _char = Maybe<HoverCharacter>.Missing();
    
    protected override void Execute(CharacterHoverChanged msg)
    {
        if (_char.IsPresent)
            _char.Value.Revert();
        
        _char = msg.HoverCharacter;
        msg.HoverCharacter.IfPresent(h =>
        {
            if (h.IsInitialized && h.Member.TeamType == TeamType.Enemies)
            {
                h.SetAction(() => { }, () =>
                {
                    h.Revert();
                    Message.Publish(new ShowEnemyDetails(state.GetEnemyById(h.Member.Id)));
                });
                h.SetIsHovered();
            }
        });
    }
}
