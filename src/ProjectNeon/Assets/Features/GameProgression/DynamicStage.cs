using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage Builder")]
public class DynamicStage : ScriptableObject
{
    [SerializeField] private GameMap2 gameMap;
    [SerializeField] private EncounterBuilder encounterBuilder;
    [SerializeField] private EncounterBuilder eliteEncounterBuilder;
    [SerializeField] private StorySetting storySetting;
    [SerializeField] private ParticleSystem.MinMaxCurve powerCurve;
    [SerializeField] private ParticleSystem.MinMaxCurve elitePowerCurve;
    [SerializeField] private GameObject[] possibleBattlegrounds;
    [SerializeField][Range(0,99)] private int segmentCount;
    [SerializeField] private GameObject bossBattlefield;
    [SerializeField] private Enemy[] bossEnemies;
    [SerializeField] private NodeTypeOdds nodeTypeOdds;

    public GameMap2 Map => gameMap;
    public EncounterBuilder EncounterBuilder => encounterBuilder;
    public EncounterBuilder EliteEncounterBuilder => eliteEncounterBuilder;
    public StorySetting StorySetting => storySetting;
    public int GetPowerLevel(float percent) => (int)Mathf.Round(powerCurve.Evaluate(percent));
    public int GetElitePowerLevel(float percent) => (int)Mathf.Round(elitePowerCurve.Evaluate(percent));
    public GameObject Battleground => possibleBattlegrounds.Random();
    public int SegmentCount => segmentCount;
    public GameObject BossBattlefield => bossBattlefield;
    public Enemy[] BossEnemies => bossEnemies;
    public MapNodeType RandomNodeType => nodeTypeOdds.GetRandomNodeType();
    public NodeTypeOdds NodeTypeOdds => nodeTypeOdds;
}