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

    //only to be used by EnemiesWantedEditor
    public Enemy[] _possible => possible;
    public float _flexibility => flexibility;
    public List<EncounterWeightsV4> _weightedComps => weightedComps;
    public List<EncounterRoleChanceV4> _chancesBasedOnEnemyNumber => chancesBasedOnEnemyNumber;
    
    public List<EnemyInstance> Generate(int difficulty, int currentChapterNumber)
    {
        var comp = GetComposition(difficulty);
        var strengthPer1 = (float)difficulty / comp.Sum(x => x.Weight);
        return comp.Select(x => SelectEnemy(x.Weight * strengthPer1, x.Role, x.IsElite)).ToList();
    }

    private List<WeightedRole> GetComposition(int difficulty)
    {
        float minDamageDealer = GetPossible(BattleRole.DamageDealer, false).Min(x => x.PowerLevel) * (1f - flexibility);
        float minProtector = GetPossible(BattleRole.Survivability, false).Min(x => x.PowerLevel) * (1f - flexibility);
        float minSpecialist = GetPossible(BattleRole.Specialist, false).Min(x => x.PowerLevel) * (1f - flexibility);
        float maxDamageDealer = GetPossible(BattleRole.DamageDealer, false).Max(x => x.PowerLevel) * (1f + flexibility);
        float maxProtector = GetPossible(BattleRole.Survivability, false).Max(x => x.PowerLevel) * (1f + flexibility);
        float maxSpecialist = GetPossible(BattleRole.Specialist, false).Max(x => x.PowerLevel) * (1f + flexibility);
        float minEliteDamageDealer = allowElites ? GetPossible(BattleRole.DamageDealer, true).Min(x => x.PowerLevel) * (1f - flexibility) : 0;
        float minEliteProtector = allowElites ? GetPossible(BattleRole.Survivability, true).Min(x => x.PowerLevel) * (1f - flexibility) : 0;
        float minEliteSpecialist = allowElites ? GetPossible(BattleRole.Specialist, true).Min(x => x.PowerLevel) * (1f - flexibility) : 0;
        float maxEliteDamageDealer = allowElites ? GetPossible(BattleRole.DamageDealer, true).Max(x => x.PowerLevel) * (1f + flexibility) : 0;
        float maxEliteProtector = allowElites ? GetPossible(BattleRole.Survivability, true).Max(x => x.PowerLevel) * (1f + flexibility) : 0;
        float maxEliteSpecialist = allowElites ? GetPossible(BattleRole.Specialist, true).Max(x => x.PowerLevel) * (1f + flexibility) : 0;

        var validWeightedComps = GetValidWeightedComps(difficulty,
            Math.Min(minDamageDealer, Math.Min(minProtector, minSpecialist)),
            Math.Max(maxDamageDealer, Math.Max(maxProtector, maxSpecialist)),
            Math.Min(minEliteDamageDealer, Math.Min(minEliteProtector, minEliteSpecialist)),
            Math.Max(maxEliteDamageDealer, Math.Max(maxEliteProtector, maxEliteSpecialist)));
        if (!validWeightedComps.Any())
        {
            Log.Error($"There was no valid {(allowElites ? "elite" : "normal")} compositions for difficulty {difficulty}");
            validWeightedComps = weightedComps.ToArray();
        }
        var options = new List<EncounterWeightsV4>();
        for (var i = 0; i < validWeightedComps.Length; i++)
            for (var ii = 0; ii < validWeightedComps[i].ChanceOfThisComp; ii++)
                options.Add(validWeightedComps[i]);
        var selectedOption = options.Random();
        var selectedOptionWeights = selectedOption.Weights.Shuffled();
        
        var result = new List<WeightedRole>();
        for (var i = 0; i < selectedOptionWeights.Length; i++)
        {
            var strength = (float)selectedOptionWeights[i] / (float)selectedOptionWeights.Max() * (float)difficulty;
            var isElite = allowElites && selectedOptionWeights.Max() == selectedOptionWeights[i];
            var validRoles = chancesBasedOnEnemyNumber[i].RollChances;
            validRoles = validRoles.Where(x =>
            {
                if (isElite && x == BattleRole.DamageDealer)
                    return strength > minEliteDamageDealer && strength < maxEliteDamageDealer;
                if (isElite && x == BattleRole.Survivability)
                    return strength > minEliteProtector && strength < maxEliteProtector;
                if (isElite && x == BattleRole.Specialist)
                    return strength > minEliteSpecialist && strength < maxEliteSpecialist;
                if (x == BattleRole.DamageDealer)
                    return strength > minDamageDealer && strength < maxDamageDealer;
                if (x == BattleRole.Survivability)
                    return strength > minProtector && strength < maxProtector;
                if (x == BattleRole.Specialist)
                    return strength > minSpecialist && strength < maxSpecialist;
                return false;
            }).ToArray();
            if (!validRoles.Any())
            {
                Log.Error($"there was no valid role for a {(isElite ? "elite" : "normal")} enemy with strength of {(int)Math.Round(strength * (1 - flexibility))} - {(int)Math.Round(strength * (1 + flexibility))}");
                validRoles = chancesBasedOnEnemyNumber[i].RollChances;
            }
            result.Add(new WeightedRole 
            { 
                Role = validRoles.Random(), 
                Weight = selectedOptionWeights[i], 
                IsElite = isElite
            });
        }
        return result;
    }

    private EncounterWeightsV4[] GetValidWeightedComps(int difficulty, float min, float max, float minElite, float maxElite)
        => allowElites 
            ? weightedComps.Where(weights 
                => (float)weights.Weights.Min() / (float)weights.Weights.Sum() * (float)difficulty > min 
                && (float)weights.Weights.Where(x => x != weights.Weights.Max()).Max() / (float)weights.Weights.Sum() * (float)difficulty < max
                && (float)weights.Weights.Max() / (float)weights.Weights.Sum() * (float)difficulty > minElite
                && (float)weights.Weights.Max() / (float)weights.Weights.Sum() * (float)difficulty < maxElite).ToArray()
            : weightedComps.Where(x 
                => (float)x.Weights.Min() / (float)x.Weights.Sum() * (float)difficulty > min 
                && (float)x.Weights.Max() / (float)x.Weights.Sum() * (float)difficulty < max).ToArray();

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
        var tieredEnemies = possible.Where(x => x.IsReadyForPlay && ((isElite && x.Tier == EnemyTier.Elite) || (!isElite && x.Tier != EnemyTier.Elite && x.Tier != EnemyTier.Boss)));
        
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
