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
    [SerializeField] private int rewardCreditsPerPowerLevel = 25;
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
    public int RewardCreditsPerPowerLevel => rewardCreditsPerPowerLevel;
    public bool IsV2 => dynamicStages != null && dynamicStages.Any();
}
