using TMPro;
using UnityEngine;

public sealed class XpAndLevelUpPresenter : MonoBehaviour
{
    [SerializeField] private XpPresenter xp;
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private TextCommandButton levelUpButton;

    public void Init(Hero h)
    {
        xp.Init(h);
        levelLabel.text = h.Level.ToString();
        levelUpButton.gameObject.SetActive(false);
        if (h.Levels.LevelUpPoints > 0)
            levelUpButton.Init("Level\nUp", () => Message.Publish(new LevelUpHero(h)));
    }
}
