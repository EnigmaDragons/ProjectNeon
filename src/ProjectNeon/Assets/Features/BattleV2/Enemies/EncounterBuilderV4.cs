using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/EncounterBuilderV4")]
public class EncounterBuilderV4 : ScriptableObject, IEncounterBuilder
{
    [SerializeField] private EncounterBuilderHistory history;
    [SerializeField] private bool allowElites = true;
    [SerializeField] private Enemy[] possible;
    [SerializeField][Range(0, 1)] private float flexibility;
    [SerializeField] private List<EncounterWeightsV4> weightedComps;
    [SerializeField] private List<EncounterRoleChanceV4> chancesBasedOnEnemyNumber;

    public List<EnemyInstance> Generate(int difficulty, int currentChapterNumber)
    {
        var comp = GetComposition();
        var strengthPer1 = difficulty / comp.Sum(x => x.Weight);
        return comp.Select(x => SelectEnemy(x.Weight * strengthPer1, x.Role, x.IsElite)).ToList();
    }

    private List<WeightedRole> GetComposition()
    {
        var options = new List<EncounterWeightsV4>();
        for (int i = 0; i < weightedComps.Count; i++)
            for (int ii = 0; ii < weightedComps[i].ChanceOfThisComp; ii++)
                options.Add(weightedComps[i]);
        var selectedOption = options.Random();
        var result = new List<WeightedRole>();
        for (var i = 0; i < selectedOption.Weights.Length; i++)
            result.Add(new WeightedRole { 
                Role = chancesBasedOnEnemyNumber[i].RollChances.Random(), 
                Weight = selectedOption.Weights[i], 
                IsElite = allowElites && selectedOption.Weights.Max(x => x) == selectedOption.Weights[i] });
        return result;
    }

    private EnemyInstance SelectEnemy(float strength, BattleRole role, bool elite)
    {
        var enemies = possible.Where(x =>
            ((elite && x.Tier == EnemyTier.Elite) || (!elite && x.Tier != EnemyTier.Elite && x.Tier != EnemyTier.Boss))
            && x.BattleRole == role);
        var withInPowerRange = enemies.Where(x => strength * (1f - flexibility) > x.stageDetails[0].powerLevel && strength * (1f + flexibility) > x.stageDetails[0].powerLevel).ToArray();
        if (withInPowerRange.Any())
            return withInPowerRange.Random().ForStage(1);
        return enemies.OrderBy(x => Math.Abs(x.stageDetails[0].powerLevel - strength)).First().ForStage(1);
    }

    private class WeightedRole
    {
        public int Weight;
        public BattleRole Role;
        public bool IsElite;
    }
}