using TMPro;
using UnityEngine;

public sealed class XpAndLevelUpPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private XpPresenter xp;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI levelLabel;
    [SerializeField] private LocalizedCommandButton levelUpButton;
    [SerializeField] private GameObject maxLevelLabel;

    private Hero _hero;
    
    private const string Levelup2Term = "Menu/LevelUp2";
    private const string MaxTerm = "Maps/Max";
    
    public void Init(Hero h)
    {
        if (h == null)
            return;
        
        _hero = h;
        Render();
    }

    private void Render()
    {
        xp.Init(_hero);
        xp.gameObject.SetActive(!_hero.IsMaxLevelV4);
        maxLevelLabel.SetActive(_hero.IsMaxLevelV4);
        levelLabel.text = _hero.Level.ToString();
        
        if (levelUpButton == null)
            return;
        
        levelUpButton.gameObject.SetActive(false);
        if (_hero.Levels.UnspentLevelUpPoints > 0)
            levelUpButton.InitTerm(Levelup2Term, () =>
            {
                Debug.Log($"Clicked Level Up {_hero.NameTerm.ToEnglish()}");
                Message.Publish(new LevelUpHero(_hero));
            });
    }

    public string[] GetLocalizeTerms()
        => new[]
        {
            Levelup2Term,
            MaxTerm
        };
}
