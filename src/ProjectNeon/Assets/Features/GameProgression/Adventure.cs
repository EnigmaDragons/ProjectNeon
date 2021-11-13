using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject
{
    [SerializeField, UnityEngine.UI.Extensions.ReadOnly] public int id;
    [SerializeField] private string lockConditionExplanation = "";
    [SerializeField] private DynamicStage[] dynamicStages;
    [SerializeField] private StaticStageV4[] stages;
    [SerializeField] private string adventureTitle;
    [SerializeField] private Sprite adventureImage;
    [SerializeField] private int partySize;
    [SerializeField] private string allowedHeroesDescription = "";
    [SerializeField] private BaseHero[] requiredHeroes;
    [SerializeField] private BaseHero[] bannedHeroes;
    [SerializeField] private int baseNumberOfCardCycles = 2;
    [SerializeField] private float rewardCreditsPerPowerLevel = 1f;
    [SerializeField] private float xpPerPowerLevel = 0.2f;
    [SerializeField, TextArea(4, 10)] private string story;
    [SerializeField, TextArea(4, 10)] private string defeatConclusion = "";
    [SerializeField, TextArea(4, 10)] private string victoryConclusion = "";

    public int Id => id;
    public string Title => adventureTitle;
    public string Story => story;

    public string DefeatConclusion => defeatConclusion;
    public string VictoryConclusion => victoryConclusion;

    public DynamicStage[] DynamicStages => dynamicStages.ToArray();
    public StaticStageV4[] StagesV4 => stages.ToArray();
    public Sprite AdventureImage => adventureImage;
    public int PartySize => partySize;
    public BaseHero[] RequiredHeroes => requiredHeroes;
    public BaseHero[] BannedHeroes => bannedHeroes ?? Array.Empty<BaseHero>();
    public int BaseNumberOfCardCycles => baseNumberOfCardCycles;
    public float RewardCreditsPerPowerLevel => rewardCreditsPerPowerLevel;
    public float XpPerPowerLevel => xpPerPowerLevel;
    public bool IsV2 => !IsV4 && dynamicStages != null && dynamicStages.Any();
    public bool IsV4 => stages != null && stages.Any();
    
    public bool IsLocked => !string.IsNullOrWhiteSpace(lockConditionExplanation);
    public string LockConditionExplanation => lockConditionExplanation ?? "";

    public string AllowedHeroesDescription => allowedHeroesDescription;
}
