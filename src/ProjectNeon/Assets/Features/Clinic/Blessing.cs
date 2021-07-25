using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Blessing
{
    public string Name;
    public HeroCharacter[] Targets;
    public EffectData Effect;
    
    public void Apply(BattleState state)
    {
        var target = new Multiple(Targets.Select(state.GetMemberByHero));
        BattleLog.Write($"Applying {Name} Blessing to {target.ToFriendlyString()}");
        var ctx = new EffectContext(
            target.Members[0], 
            target,
            Maybe<Card>.Missing(), 
            ResourceQuantity.None, 
            state.Party, 
            state.PlayerState, 
            state.Members,
            state.PlayerCardZones,
            new UnpreventableContext(), 
            new SelectionContext(), 
            new Dictionary<int, CardTypeData>(),
            state.Party.Credits,
            state.Party.Credits, 
            new Dictionary<int, EnemyType>(), 
            state.GetNextCardId, 
            new PlayedCardSnapshot[0]);
        AllEffects.Apply(Effect, ctx);
    }
}