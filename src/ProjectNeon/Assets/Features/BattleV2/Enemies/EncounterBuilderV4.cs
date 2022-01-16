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

    //only to be used by EnemiessWantedEditor
    public Enemy[] _possible => possible;
    public float _flexibility => flexibility;
    public List<EncounterWeightsV4> _weightedComps => weightedComps;
    public List<EncounterRoleChanceV4> _chancesBasedOnEnemyNumber => chancesBasedOnEnemyNumber;
    
    public List<EnemyInstance> Generate(int difficulty, int currentChapterNumber)
    {
        var comp = GetComposition();
        var strengthPer1 = (float)difficulty / comp.Sum(x => x.Weight);
        return comp.Select(x => SelectEnemy(x.Weight * strengthPer1, x.Role, x.IsElite)).ToList();
    }

    private List<WeightedRole> GetComposition()
    {
        var options = new List<EncounterWeightsV4>();
        for (var i = 0; i < weightedComps.Count; i++)
            for (var ii = 0; ii < weightedComps[i].ChanceOfThisComp; ii++)
                options.Add(weightedComps[i]);
        var selectedOption = options.Random();
        
        var result = new List<WeightedRole>();
        for (var i = 0; i < selectedOption.Weights.Length; i++)
            result.Add(new WeightedRole 
            { 
                Role = chancesBasedOnEnemyNumber[i].RollChances.Random(), 
                Weight = selectedOption.Weights[i], 
                IsElite = allowElites && selectedOption.Weights.Max(x => x) == selectedOption.Weights[i] 
            });
        return result;
    }

    private EnemyInstance SelectEnemy(float strength, BattleRole role, bool isElite)
    {
        var enemyPool = GetPossible(role, isElite);
        var minPower = strength * (1f - flexibility);
        var maxPower = strength * (1f + flexibility);
        
        var withInPowerRange = enemyPool.Where(x => x.PowerLevel >= minPower && x.PowerLevel <= maxPower).ToArray();
        if (withInPowerRange.Any())
            return withInPowerRange.Random();
        
        Log.Warn($"Missing Content: Couldn't find {(isElite ? "elite" : "normal")} enemy with the {role} role in the power range between {minPower} - {maxPower}");
        return enemyPool.OrderBy(x => Math.Abs(x.PowerLevel - strength)).First();
    }

    private EnemyInstance[] GetPossible(BattleRole role, bool isElite)
    {
        var tieredEnemies = possible.Where(x => (isElite && x.Tier == EnemyTier.Elite) || (!isElite && x.Tier != EnemyTier.Elite && x.Tier != EnemyTier.Boss));
        
        var roleMatchingEnemies = tieredEnemies
            .Where(x => x.BattleRole == role)
            .Select(x => x.ForStage(1))
            .ToArray();
        
        if (roleMatchingEnemies.Any())
            return roleMatchingEnemies;
        
        Log.Warn($"Missing Content: Couldn't find {(isElite ? "elite" : "normal")} enemy with the {role} role");
        return tieredEnemies
            .Select(x => x.ForStage(1))
            .ToArray();
    }

    private class WeightedRole
    {
        public int Weight;
        public BattleRole Role;
        public bool IsElite;
    }
}
