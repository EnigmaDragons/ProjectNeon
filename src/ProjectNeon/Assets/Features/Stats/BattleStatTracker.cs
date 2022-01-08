using System.Linq;
using UnityEngine;

public class BattleStatTracker : OnMessage<EffectResolved>
{
    [SerializeField] private BattleState state;
    
    protected override void Execute(EffectResolved msg)
    {
        UpdatePartyChangeStats(msg);
        UpdateEnemyChangeStats(msg);
    }

    private void UpdatePartyChangeStats(EffectResolved e)
    {
        var partyBefore = e.BattleBefore.Members.Where(m => m.Value.TeamType == TeamType.Party);
        var partyAfter = e.BattleAfter.Members.Where(m => m.Value.TeamType == TeamType.Party);

        var damageReceived = partyAfter.Sum(x => x.Value.State.HpAndShield) - partyBefore.Sum(x => x.Value.State.HpAndShield);
        if (damageReceived < 0)
            state.Stats.DamageReceived -= damageReceived;
        
        var hpChange = partyAfter.Sum(x => x.Value.State.Hp) - partyBefore.Sum(x => x.Value.State.Hp);
        if (hpChange < 0)
            state.Stats.HpDamageReceived -= hpChange;
        else
            state.Stats.HealingReceived += hpChange;
    }

    private void UpdateEnemyChangeStats(EffectResolved e)
    {
        var enemiesBefore = e.BattleBefore.Members.Where(m => m.Value.TeamType == TeamType.Enemies);
        var enemiesAfter = e.BattleAfter.Members.Where(m => m.Value.TeamType == TeamType.Enemies);

        var damageDealt = enemiesAfter.Sum(x => x.Value.State.HpAndShield) - enemiesBefore.Sum(x => x.Value.State.HpAndShield);
        if (damageDealt < 0)
            state.Stats.DamageDealt -= damageDealt;
    }
}
