using System;
using System.Linq;
using UnityEngine;

public class ChooseBuyoutCardOrDefaultToCreate : Effect
{
    private readonly int _template;
    private readonly int[] _otherOptions;

    public ChooseBuyoutCardOrDefaultToCreate(string effectContext)
    {
        _otherOptions = effectContext.Split(',').Select(x => int.Parse(x)).ToArray();
        _template = _otherOptions[0];
        _otherOptions = _otherOptions.Skip(1).ToArray();
    }
    
    public void Apply(EffectContext ctx)
    {
        var cardTemplate = ctx.AllCards[_template];
        ctx.Selections.CardSelectionOptions = ctx.BattleMembers
            .Where(x => x.Value.TeamType == TeamType.Enemies && x.Value.IsConscious())
            .Select(x => new Card(-1, ctx.Source, new InMemoryCard()
            {
                Name = cardTemplate.Name,
                Rarity = cardTemplate.Rarity,
                Cost = new InMemoryResourceAmount(CalculatePrice(x.Value, ctx.EnemyTypes[x.Key]), "Creds"),
                Gain = cardTemplate.Gain,
                Speed = cardTemplate.Speed,
                //ActionSequences = ,
                Archetypes = cardTemplate.Archetypes,
                IsSinglePlay = true,
                Art = cardTemplate.Art,
                Description = $"{ctx.EnemyTypes[x.Key].Name} is paid off to leave the battle.",
                Tags = cardTemplate.Tags,
                TypeDescription = cardTemplate.TypeDescription
            }))
            .ToArray();
        ctx.Selections.OnCardSelected = card => ctx.PlayerCardZones.HandZone.PutOnBottom(card);
    }

    private int CalculatePrice(Member enemy, EnemyType type)
    {
        var percentage = (enemy.CurrentHp() + enemy.CurrentShield()) /
                         (enemy.MaxHp() + enemy.State[StatType.StartingShield]);
        if (type.Tier == EnemyTier.Boss)
        {
            if (enemy.State[TemporalStatType.Phase] == 1)
                return Mathf.CeilToInt((percentage + 5) * type.PowerLevel);
            else if (enemy.State[TemporalStatType.Phase] == 2)
                return Mathf.CeilToInt((percentage + 2) * type.PowerLevel);
            else if (enemy.State[TemporalStatType.Phase] == 3)
                return Mathf.CeilToInt(percentage * type.PowerLevel);
            else
                return 999999;
        }
        else if (type.Tier == EnemyTier.Elite)
            return Mathf.CeilToInt(percentage * type.PowerLevel * 1.5f);
        else if (type.Tier == EnemyTier.Minion)
            return Mathf.CeilToInt(percentage * type.PowerLevel * 0.5f);
        return Mathf.CeilToInt(percentage * type.PowerLevel * 1);
    }
}