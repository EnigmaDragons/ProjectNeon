using TMPro;
using UnityEngine;

public sealed class XpAndLevelUpPresenter : MonoBehaviour
{
    [SerializeField] private XpPresenter xp;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI levelLabel;
    [SerializeField] private LocalizedCommandButton levelUpButton;

    private Hero _hero;
    
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
            levelUpButton.InitTerm("Menu/LevelUp2", () =>
            {
                Debug.Log($"Clicked Level Up {_hero.NameTerm.ToEnglish()}");
                Message.Publish(new LevelUpHero(_hero));
            });
    }
}
