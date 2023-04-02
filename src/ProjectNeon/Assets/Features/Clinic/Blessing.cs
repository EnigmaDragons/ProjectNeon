using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Blessing
{
    public string Name;
    public BaseHero[] Targets;
    public EffectData Effect;
    public int Duration;
    
    public Blessing() {}
    
    public void Apply(BattleState state)
    {
        var targetMaybes = Targets.Select(t => state.GetMaybeMemberByHeroCharacterId(t.Id)).ToArray();
        var presentTargets = targetMaybes.Where(x => x.IsPresent).Select(x => x.Value).ToArray();
        if (targetMaybes.Length > presentTargets.Length)
            Log.Error($"Non-Crashing: Blessing {Name} had {targetMaybes.Length} expected targets, and only found {presentTargets.Length} matching targets in Battle State Heroes. Likely a serialization issue.");

        if (presentTargets.Length == 0)
        {
            BattleLog.Write($"Unable to apply Blessing {Name} due to developer error. Open a bug ticket.");
            return;
        }
        
        var target = new Multiple(presentTargets);
        BattleLog.Write($"Applying {Name} Blessing to {target.ToFriendlyString()}");
        var ctx = new EffectContext(
            target.Members[0], 
            target,
            Maybe<Card>.Missing(), 
            ResourceQuantity.None, 
            ResourceQuantity.None, 
            state.Party, 
            state.PlayerState, 
            state.RewardState,
            state.Members,
            state.PlayerCardZones,
            new UnpreventableContext(), 
            new SelectionContext(), 
            new Dictionary<int, CardType>(),
            state.Party.Credits,
            state.Party.Credits, 
            new Dictionary<int, EnemyType>(), 
            state.GetNextCardId, 
            new PlayedCardSnapshot[0],
            state.OwnerTints,
            state.OwnerBusts,
            true,
            ReactionTimingWindow.FirstCause, 
            new EffectScopedData(),
            new DoubleDamageContext(target.Members[0], false));
        AllEffects.Apply(Effect, ctx);
    }
}