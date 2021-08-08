using System.Linq;
using UnityEngine;

public class ChooseBuyoutCardOrDefaultToCreate : Effect
{
    private readonly int _template;
    private readonly int[] _otherOptions;

    public ChooseBuyoutCardOrDefaultToCreate(string effectContext)
    {
        _otherOptions = string.IsNullOrWhiteSpace(effectContext) 
            ? new int[0] 
            : effectContext.Split(',').Select(x => int.Parse(x)).ToArray();
        _template = _otherOptions.Any() 
            ? _otherOptions[0] 
            : -1;
        _otherOptions = _otherOptions.Skip(1).ToArray();
    }
    
    public void Apply(EffectContext ctx)
    {
        var cardTemplate = ctx.AllCards[_template];
        ctx.Selections.CardSelectionOptions = ctx.BattleMembers
            .Where(x => x.Value.TeamType == TeamType.Enemies && x.Value.IsConscious())
            .Select(x => new Card(ctx.GetNextCardId(), ctx.Source, new InMemoryCard()
                {
                    Name = cardTemplate.Name,
                    Rarity = cardTemplate.Rarity,
                    Cost = new InMemoryResourceAmount(CalculatePrice(x.Value, ctx.EnemyTypes[x.Key]), "Creds"),
                    Speed = cardTemplate.Speed,
                    ActionSequences = new CardActionSequence[]
                    {
                        cardTemplate.ActionSequences[0].CloneForBuyout(new EffectData
                        {
                            EffectType = EffectType.BuyoutEnemyById, EffectScope = new StringReference(x.Key.ToString())
                        })
                    },
                    Archetypes = cardTemplate.Archetypes,
                    IsSinglePlay = true,
                    Art = cardTemplate.Art,
                    Description = $"{ctx.EnemyTypes[x.Key].Name} is paid off to leave the battle.",
                    Tags = cardTemplate.Tags,
                    TypeDescription = cardTemplate.TypeDescription
                }, 
                ctx.OwnerTints.ContainsKey(ctx.Source.Id) ? ctx.OwnerTints[ctx.Source.Id] : Maybe<Color>.Missing(), 
                ctx.OwnerBusts.ContainsKey(ctx.Source.Id) ? ctx.OwnerBusts[ctx.Source.Id] : Maybe<Sprite>.Missing()))
            .Concat(_otherOptions.Select(x => new Card(ctx.GetNextCardId(), ctx.Source, ctx.AllCards[x],
                ctx.OwnerTints.ContainsKey(ctx.Source.Id) ? ctx.OwnerTints[ctx.Source.Id] : Maybe<Color>.Missing(), 
                ctx.OwnerBusts.ContainsKey(ctx.Source.Id) ? ctx.OwnerBusts[ctx.Source.Id] : Maybe<Sprite>.Missing())))
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
            return Mathf.CeilToInt(percentage * type.PowerLevel * 1.4f);
        else if (type.Tier == EnemyTier.Minion)
            return Mathf.CeilToInt(percentage * type.PowerLevel * 0.35f);
        return Mathf.CeilToInt(percentage * type.PowerLevel * 0.7f);
    }
}