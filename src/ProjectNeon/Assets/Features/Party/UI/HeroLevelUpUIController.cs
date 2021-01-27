using TMPro;
using UnityEngine;

public class HeroLevelUpUIController : OnMessage<PartyAdventureStateChanged, LevelUpHero, Finished<LevelUpHero>>
{
    [SerializeField] private GameObject view;
    [SerializeField] private HeroDetailsPanel details;
    [SerializeField] private HeroFlexibleLevelUpPresenter levelUpPresenter;
    [SerializeField] private TextMeshProUGUI remainingPointsLabel;
    
    private Hero _hero;

    protected override void Execute(PartyAdventureStateChanged msg)
    {
        if (_hero != null)
            UpdateUi();
    }

    protected override void Execute(LevelUpHero msg)
    {
        _hero = msg.Hero;
        UpdateUi();
        view.SetActive(true);
    }

    private void UpdateUi()
    {
        details.Initialized(_hero, canInteractWithEquipment: false);
        levelUpPresenter.Initialize(_hero);
        remainingPointsLabel.text = $"{_hero.LevelUpPoints} Stat Point(s)";
    }

    protected override void Execute(Finished<LevelUpHero> msg)
    {
        _hero = null;
        view.SetActive(false);
    }
}
