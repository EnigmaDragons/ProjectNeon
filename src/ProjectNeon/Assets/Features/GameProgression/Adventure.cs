using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Adventure/Adventure")]
public class Adventure : ScriptableObject
{
    [SerializeField] private StageBuilder[] stages;
    [SerializeField] private string adventureTitle;
    [SerializeField] private Sprite adventureImage;
    [SerializeField] private int partySize;
    [SerializeField] private BaseHero[] requiredHeroes;
    [SerializeField] private int baseNumberOfCardCycles = 2;
    [SerializeField] private bool canUseUniversalShop = true;
    [SerializeField, TextArea(4, 10)] private string story;

    // @todo #1:15min Design. What happens when the adventure is won?

    public string Title => adventureTitle;
    public string Story => story;
    public StageBuilder[] Stages => stages.ToArray();
    public Sprite AdventureImage => adventureImage;
    public int PartySize => partySize;
    public BaseHero[] RequiredHeroes => requiredHeroes;
    public int BaseNumberOfCardCycles => baseNumberOfCardCycles;
    public bool CanUseUniversalShop => canUseUniversalShop;
}
