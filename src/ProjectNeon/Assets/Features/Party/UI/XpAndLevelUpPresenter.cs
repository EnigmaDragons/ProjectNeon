using TMPro;
using UnityEngine;

public sealed class XpAndLevelUpPresenter : MonoBehaviour
{
    [SerializeField] private XpPresenter xp;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextCommandButton levelUpButton;

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
        Debug.Log($"Render Xp And Level Up Button {_hero.Name} - Level Up Points {_hero.Levels.LevelUpPoints}");
        xp.Init(_hero);
        levelLabel.text = _hero.Level.ToString();
        levelUpButton.gameObject.SetActive(false);
        if (_hero.Levels.LevelUpPoints > 0)
            levelUpButton.Init("Level\nUp", () =>
            {
                Debug.Log($"Clicked Level Up {_hero.Name}");
                Message.Publish(new LevelUpHero(_hero));
            });
    }
}
