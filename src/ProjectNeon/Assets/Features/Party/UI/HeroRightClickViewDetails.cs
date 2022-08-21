using UnityEngine;

public class HeroRightClickViewDetails : OnMessage<CharacterHoverChanged>
{
    [SerializeField] private PartyAdventureState party;

    private Maybe<HoverCharacter> _char = Maybe<HoverCharacter>.Missing();
    
    protected override void Execute(CharacterHoverChanged msg)
    {
        if (_char.IsPresent)
            _char.Value.Revert();
        
        _char = msg.HoverCharacter;
        msg.HoverCharacter.IfPresent(h =>
        {
            if (!h.IsInitialized || h.Member.TeamType != TeamType.Party) return;
            
            var maybeHero = party.Heroes.FirstOrMaybe(hero => hero.Name.Equals(h.Member.Name));
            maybeHero.IfPresent(hero =>
            {
                h.SetAction(() => { }, () =>
                {
                    h.Revert();
                    Message.Publish(new ShowHeroDetailsView(hero, h.Member));
                    Message.Publish(new ShowEnemySFX(transform));
                });
                h.SetIsHovered();
            });
        });
    }
}