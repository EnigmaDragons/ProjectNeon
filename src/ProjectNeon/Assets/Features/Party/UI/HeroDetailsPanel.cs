using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class HeroDetailsPanel : OnMessage<HeroStateChanged>, ILocalizeTerms
{
    [SerializeField] private Image heroBust;
    [SerializeField] private Localize nameLocalize;
    [SerializeField] private Localize classLocalize;
    [SerializeField] private MemberStatPanel stats;
    [SerializeField] private HeroEquipmentPanel equipment;
    [SerializeField] private HeroInjuryPanel injuries;
    [SerializeField] private LocalizedCommandButton levelUpButton;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI levelLabel;

    private Hero _hero;
    private bool _canInteractWithEquipment;
    
    private const string LevelUpTerm = "Menu/LevelUp";
    
    public HeroDetailsPanel Initialized(Hero h, bool canInteractWithEquipment)
    {
        _hero = h;
        _canInteractWithEquipment = canInteractWithEquipment;
        nameLocalize.SetTerm(h.NameTerm);
        classLocalize.SetTerm(h.ClassTerm);
        levelLabel.text = h.Level.ToString();
        heroBust.sprite = h.Character.Bust;
        stats.Initialized(h);
        injuries.Init(h);
        if (equipment != null)
            equipment.Initialized(h, !canInteractWithEquipment);
        
        levelUpButton?.gameObject.SetActive(false);
        if (levelUpButton != null && !h.IsMaxLevelV4 && h.Levels.UnspentLevelUpPoints > 0)
            levelUpButton.InitTerm(LevelUpTerm, () => Message.Publish(new LevelUpHero(h)));
        
        return this;
    }

    protected override void Execute(HeroStateChanged msg) => Initialized(_hero, _canInteractWithEquipment);

    public string[] GetLocalizeTerms()
        => new[] {LevelUpTerm};
}
