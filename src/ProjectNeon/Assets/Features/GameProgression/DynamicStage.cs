﻿using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Stage Builder")]
public class DynamicStage : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private GameMap2 gameMap;
    [SerializeField] private GameMap3 gameMap3;
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
    [SerializeField] private NodeTypeOdds2 nodeTypeOdds2;
    [SerializeField] private AudioClipVolume stageBattleTheme;
    [SerializeField] private StageRarityFactors rewardRarityFactors;

    public string DisplayName => displayName;
    public GameMap3 Map => gameMap3;
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
    public NodeTypeOdds2 NodeTypeOdds2 => nodeTypeOdds2;
    public AudioClipVolume StageBattleTheme => stageBattleTheme;
    public RarityFactors RewardRarityFactors => rewardRarityFactors != null 
        ? rewardRarityFactors 
        : (RarityFactors)new DefaultRarityFactors();
}