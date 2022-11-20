using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage V5")]
public class HybridStageV5 : ScriptableObject, IStage
{
    [SerializeField] public int id;
    [SerializeField] public string displayName;
    [SerializeField] private GameMap3 gameMap3;
    [SerializeField] private EncounterBuilderV5 encounterBuilder;
    [SerializeField] private StorySetting storySetting;
    [SerializeField] private PowerCurve powerCurve;
    [SerializeField] private PowerCurve elitePowerCurve;
    [SerializeField] private SegmentRangeBattlefieldSet[] battlefieldSets; 
    [SerializeField] private StageSegment[] primarySegments;
    [SerializeField] private StageSegment[] maybeSecondarySegments;
    [SerializeField] private StageSegment[] maybeStorySegments;
    [SerializeField] private GameObject bossBattlefield;
    [SerializeField] private Enemy[] bossEnemies;
    [SerializeField] private StageRarityFactors rewardRarityFactors;
    [SerializeField] private float shopOdds = 0.17f;
    [SerializeField] private int noShopsUntilSegment = 3;

    public string DisplayName => $"Stage/Stage{id}Name";

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
    public IEncounterBuilder EliteEncounterBuilder => encounterBuilder;
    public StorySetting StorySetting => storySetting;
    public float ShopOdds => shopOdds;
    public int NoShopUntilSegment => noShopsUntilSegment;
    public int GetPowerLevel(float percent) => powerCurve.GetValueAsInt(percent);
    public int GetElitePowerLevel(float percent) => elitePowerCurve.GetValueAsInt(percent);
    private int CombatSegmentCount => primarySegments.Count(s => s.ShouldCountTowardsEnemyPowerLevel);
    public float CombatProgress(int completedSegments) => primarySegments.Take(completedSegments).Count(s => s.ShouldCountTowardsEnemyPowerLevel) / (float)CombatSegmentCount;
    public GameObject Battleground => BattlegroundForSegment(0);
    public int SegmentCount => primarySegments.Length;
    public int SegmentCountToBoss => primarySegments.TakeWhile(s => s.MapNodeType != MapNodeType.Boss).Count();
    public StageSegment[] Segments => primarySegments;
    public StageSegment[] MaybeSecondarySegments => maybeSecondarySegments;
    public StageSegment[] MaybeStorySegments => maybeStorySegments;
    public GameObject BossBattlefield => bossBattlefield;
    public Enemy[] BossEnemies => bossEnemies;
    public RarityFactors RewardRarityFactors => rewardRarityFactors != null 
        ? rewardRarityFactors 
        : (RarityFactors)new DefaultRarityFactors();
}