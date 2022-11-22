using TMPro;
using UnityEngine;

public sealed class XpAndLevelUpPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private XpPresenter xp;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI levelLabel;
    [SerializeField] private LocalizedCommandButton levelUpButton;

    private Hero _hero;
    
    private const string Levelup2Term = "Menu/LevelUp2";
    
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
        => new[] {Levelup2Term};
}
