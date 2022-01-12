using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage V4")]
public class StaticStageV4 : ScriptableObject, IStage
{
    [SerializeField] private string displayName;
    [SerializeField] private EncounterBuilderV4 encounterBuilder;
    [SerializeField] private EncounterBuilderV4 eliteEncounterBuilder;
    [SerializeField] private StorySetting storySetting;
    [SerializeField] private PowerCurve powerCurve;
    [SerializeField] private PowerCurve elitePowerCurve;
    [SerializeField] private SegmentRangeBattlefieldSet[] battlefieldSets; 
    [SerializeField] private int repeatPlayStartingSegmentIndex;
    [SerializeField] private StageSegment[] segments;
    [SerializeField] private GameObject bossBattlefield;
    [SerializeField] private Enemy[] bossEnemies;
    [SerializeField] private AudioClipVolume stageBattleTheme;
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

    public IEncounterBuilder EncounterBuilder => encounterBuilder;
    public IEncounterBuilder EliteEncounterBuilder => eliteEncounterBuilder;
    public StorySetting StorySetting => storySetting;
    public int GetPowerLevel(float percent) => powerCurve.GetValueAsInt(percent);
    public int GetElitePowerLevel(float percent) => elitePowerCurve.GetValueAsInt(percent);
    private int CombatSegmentCount => segments.Count(s => s.ShouldCountTowardsEnemyPowerLevel);
    public float CombatProgress(int completedSegments) => segments.Take(completedSegments).Count(s => s.ShouldCountTowardsEnemyPowerLevel) / (float)CombatSegmentCount;
    public GameObject Battleground => BattlegroundForSegment(0);
    public int SegmentCount => segments.Length;
    public int RepeatPlayStartingSegmentIndex => repeatPlayStartingSegmentIndex;
    public StageSegment[] Segments => segments;
    public GameObject BossBattlefield => bossBattlefield;
    public Enemy[] BossEnemies => bossEnemies;
    public AudioClipVolume StageBattleTheme => stageBattleTheme;
    public RarityFactors RewardRarityFactors => rewardRarityFactors != null 
        ? rewardRarityFactors 
        : (RarityFactors)new DefaultRarityFactors();
}