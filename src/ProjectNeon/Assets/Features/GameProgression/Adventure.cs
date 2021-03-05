using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject
{
    [SerializeField] private Stage[] stages;
    [SerializeField] private DynamicStage[] dynamicStages;
    [SerializeField] private string adventureTitle;
    [SerializeField] private Sprite adventureImage;
    [SerializeField] private int partySize;
    [SerializeField] private BaseHero[] requiredHeroes;
    [SerializeField] private int baseNumberOfCardCycles = 2;
    [SerializeField] private float rewardCreditsPerPowerLevel = 1f;
    [SerializeField] private float xpPerPowerLevel = 0.2f;
    [SerializeField, TextArea(4, 10)] private string story;

    // @todo #1:15min Design. What happens when the adventure is won?

    public string Title => adventureTitle;
    public string Story => story;
    public Stage[] Stages => stages.ToArray();
    public DynamicStage[] DynamicStages => dynamicStages.ToArray();
    public Sprite AdventureImage => adventureImage;
    public int PartySize => partySize;
    public BaseHero[] RequiredHeroes => requiredHeroes;
    public int BaseNumberOfCardCycles => baseNumberOfCardCycles;
    public float RewardCreditsPerPowerLevel => rewardCreditsPerPowerLevel;
    public float XpPerPowerLevel => xpPerPowerLevel;
    public bool IsV2 => dynamicStages != null && dynamicStages.Any();
}
