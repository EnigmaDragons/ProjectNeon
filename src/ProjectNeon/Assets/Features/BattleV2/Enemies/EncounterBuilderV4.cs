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
        var rng = new DeterministicRng(ConsumableRngSeed.Consume());
        var comp = GetComposition(difficulty, rng);
        var strengthPer1 = (float)difficulty / comp.Sum(x => x.Weight);
        return comp.Select(x => SelectEnemy(x.Weight * strengthPer1, x.Role, x.IsElite, rng)).ToList();
    }

    private List<WeightedRole> GetComposition(int difficulty, DeterministicRng rng)
    {
        var damageDealers = GetPossible(BattleRole.DamageDealer, false);
        var protectors = GetPossible(BattleRole.Survivability, false);
        var specialists = GetPossible(BattleRole.Specialist, false);
        var eliteDamageDealers = GetPossible(BattleRole.DamageDealer, true);
        var eliteProtectors = GetPossible(BattleRole.Survivability, true);
        var eliteSpecialists = GetPossible(BattleRole.Specialist, true);

        float minDamageDealer = damageDealers.Any() ? damageDealers.Min(x => x.PowerLevel) * (1f - flexibility) : 0f;
        float minProtector = protectors.Any() ? protectors.Min(x => x.PowerLevel) * (1f - flexibility) : 0f;
        float minSpecialist = specialists.Any() ? specialists.Min(x => x.PowerLevel) * (1f - flexibility) : 0f;
        float maxDamageDealer = damageDealers.Any() ? damageDealers.Max(x => x.PowerLevel) * (1f + flexibility) : 0f;
        float maxProtector = protectors.Any() ? protectors.Max(x => x.PowerLevel) * (1f + flexibility) : 0f;
        float maxSpecialist = specialists.Any() ? specialists.Max(x => x.PowerLevel) * (1f + flexibility) : 0f;
        float minEliteDamageDealer = allowElites && eliteDamageDealers.Any() ? eliteDamageDealers.Min(x => x.PowerLevel) * (1f - flexibility) : 0f;
        float minEliteProtector = allowElites && eliteProtectors.Any() ? eliteProtectors.Min(x => x.PowerLevel) * (1f - flexibility) : 0f;
        float minEliteSpecialist = allowElites && eliteSpecialists.Any() ? eliteSpecialists.Min(x => x.PowerLevel) * (1f - flexibility) : 0f;
        float maxEliteDamageDealer = allowElites && eliteDamageDealers.Any() ? eliteDamageDealers.Max(x => x.PowerLevel) * (1f + flexibility) : 0f;
        float maxEliteProtector = allowElites && eliteProtectors.Any() ? eliteProtectors.Max(x => x.PowerLevel) * (1f + flexibility) : 0f;
        float maxEliteSpecialist = allowElites && eliteSpecialists.Any() ? eliteSpecialists.Max(x => x.PowerLevel) * (1f + flexibility) : 0f;

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
        var selectedOption = options.Random(rng);
        var selectedOptionWeights = selectedOption.Weights.Shuffled(rng);
        
        var result = new List<WeightedRole>();
        for (var i = 0; i < selectedOptionWeights.Length; i++)
        {
            var strength = (float)selectedOptionWeights[i] / (float)selectedOptionWeights.Sum() * (float)difficulty;
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
                Role = validRoles.Random(rng), 
                Weight = selectedOptionWeights[i], 
                IsElite = isElite
            });
        }
        return result;
    }

    private EncounterWeightsV4[] GetValidWeightedComps(int difficulty, float min, float max, float minElite, float maxElite)
        => allowElites 
            ? weightedComps.Where(weights 
                => weights.Weights.Length == 1 
                || (   (float)weights.Weights.Min() / (float)weights.Weights.Sum() * (float)difficulty > min 
                    && (float)weights.Weights.Where(x => x != weights.Weights.Max()).Max() / (float)weights.Weights.Sum() * (float)difficulty < max
                    && (float)weights.Weights.Max() / (float)weights.Weights.Sum() * (float)difficulty > minElite
                    && (float)weights.Weights.Max() / (float)weights.Weights.Sum() * (float)difficulty < maxElite)).ToArray()
            : weightedComps.Where(x 
                => (float)x.Weights.Min() / (float)x.Weights.Sum() * (float)difficulty > min 
                && (float)x.Weights.Max() / (float)x.Weights.Sum() * (float)difficulty < max).ToArray();

    private EnemyInstance SelectEnemy(float strength, BattleRole role, bool isElite, DeterministicRng rng)
    {
        var enemyPool = GetPossible(role, isElite);
        var minPower = strength * (1f - flexibility);
        var maxPower = strength * (1f + flexibility);
        
        var withInPowerRange = enemyPool.Where(x => x.PowerLevel >= minPower && x.PowerLevel <= maxPower).ToArray();
        if (withInPowerRange.Any())
            return withInPowerRange.Random(rng);
        
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
