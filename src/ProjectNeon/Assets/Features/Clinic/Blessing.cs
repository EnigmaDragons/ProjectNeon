using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Blessing
{
    public Hero[] Targets;
    public EffectData Effect;
    
    public void Apply(BattleState state)
    {
        var target = Targets.Length == 1 
            ? (Target)new Single(state.GetMemberByHero(Targets[0].Character)) 
            : (Target)new Multiple(Targets.Select(x => state.GetMemberByHero(x.Character)));
        AllEffects.Apply(Effect, new EffectContext(target.Members[0], target, 
            Maybe<Card>.Missing(), ResourceQuantity.None, state.Party, state.PlayerState, state.Members, state.PlayerCardZones,
            new UnpreventableContext(), new SelectionContext(), new Dictionary<int, CardTypeData>(), state.Party.Credits,
            state.Party.Credits, new Dictionary<int, EnemyType>(), state.GetNextCardId, new PlayedCardSnapshot[0]));
    }
}