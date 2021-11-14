using UnityEngine;
using UnityEngine.UI.Extensions;

[CreateAssetMenu(menuName = "Adventure/Stage V4")]
public class StaticStageV4 : ScriptableObject, IStage
{
    [SerializeField] private string displayName;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private EncounterBuilder eliteEncounterBuilder;
    [SerializeField] private StorySetting storySetting;
    [SerializeField] private PowerCurve powerCurve;
    [SerializeField] private PowerCurve elitePowerCurve;
    [SerializeField] private GameObject[] possibleBattlegrounds;
    [SerializeField] private StageSegment[] segments;
    [SerializeField] private GameObject bossBattlefield;
    [SerializeField] private Enemy[] bossEnemies;
    [SerializeField] private AudioClipVolume stageBattleTheme;
    [SerializeField] private StageRarityFactors rewardRarityFactors;

    public string DisplayName => displayName;
    public EncounterBuilder EncounterBuilder => encounterBuilder;
    public EncounterBuilder EliteEncounterBuilder => eliteEncounterBuilder;
    public StorySetting StorySetting => storySetting;
    public int GetPowerLevel(float percent) => powerCurve.GetValueAsInt(percent);
    public int GetElitePowerLevel(float percent) => elitePowerCurve.GetValueAsInt(percent);
    public GameObject Battleground => possibleBattlegrounds.Random();
    public int SegmentCount => segments.Length;
    public StageSegment[] Segments => segments;
    public GameObject BossBattlefield => bossBattlefield;
    public Enemy[] BossEnemies => bossEnemies;
    public AudioClipVolume StageBattleTheme => stageBattleTheme;
    public RarityFactors RewardRarityFactors => rewardRarityFactors != null 
        ? rewardRarityFactors 
        : (RarityFactors)new DefaultRarityFactors();
}