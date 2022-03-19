using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage V5")]
public class HybridStageV5 : ScriptableObject, IStage
{
    [SerializeField] private string displayName;
    [SerializeField] private GameMap3 gameMap3;
    [SerializeField] private EncounterBuilderV4 encounterBuilder;
    [SerializeField] private EncounterBuilderV4 eliteEncounterBuilder;
    [SerializeField] private StorySetting storySetting;
    [SerializeField] private PowerCurve powerCurve;
    [SerializeField] private PowerCurve elitePowerCurve;
    [SerializeField] private SegmentRangeBattlefieldSet[] battlefieldSets; 
    [SerializeField] private StageSegment[] primarySegments;
    [SerializeField] private StageSegment[] maybeSecondarySegments;
    [SerializeField] private GameObject bossBattlefield;
    [SerializeField] private Enemy[] bossEnemies;
    [SerializeField] private StageRarityFactors rewardRarityFactors;

    public string DisplayName => displayName;

    public GameObject BattlegroundForSegment(int segment)
    {
        var possibleSets = battlefieldSets
            .OrderByDescending(b => b.StartsAtSegmentIndex)
            .Where(s => segment >= s.StartsAtSegmentIndex);
        var set = possibleSets.First();
        return set.GetNext();
    }

    public GameMap3 Map => gameMap3;
    public IEncounterBuilder EncounterBuilder => encounterBuilder;
    public IEncounterBuilder EliteEncounterBuilder => eliteEncounterBuilder;
    public StorySetting StorySetting => storySetting;
    public int GetPowerLevel(float percent) => powerCurve.GetValueAsInt(percent);
    public int GetElitePowerLevel(float percent) => elitePowerCurve.GetValueAsInt(percent);
    private int CombatSegmentCount => primarySegments.Count(s => s.ShouldCountTowardsEnemyPowerLevel);
    public float CombatProgress(int completedSegments) => primarySegments.Take(completedSegments).Count(s => s.ShouldCountTowardsEnemyPowerLevel) / (float)CombatSegmentCount;
    public GameObject Battleground => BattlegroundForSegment(0);
    public int SegmentCount => primarySegments.Length;
    public StageSegment[] Segments => primarySegments;
    public StageSegment[] MaybeSecondarySegments => maybeSecondarySegments;
    public GameObject BossBattlefield => bossBattlefield;
    public Enemy[] BossEnemies => bossEnemies;
    public RarityFactors RewardRarityFactors => rewardRarityFactors != null 
        ? rewardRarityFactors 
        : (RarityFactors)new DefaultRarityFactors();
}