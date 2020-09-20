using UnityEngine;

public class HeroFlexibleLevelUpPresenter : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextCommandButton[] buttons;

    private readonly HeroLevelUp _levelUp = new HeroLevelUp();
    private Hero _hero;
    
    public void Initialize(Hero hero)
    {
        _hero = hero;
        InitButton(buttons[0], StatType.MaxHP);
        InitButton(buttons[1], StatType.Toughness);
        InitButton(buttons[2], StatType.Attack);
        InitButton(buttons[3], StatType.Magic);
        InitButton(buttons[4], StatType.Armor);
        InitButton(buttons[5], StatType.Resistance);
    }
    
    private string ButtonName(StatType s) => $"+{_levelUp.Increases[s]} {s.ToString()}";

    private void InitButton(TextCommandButton b, StatType s)
        => b.Init(ButtonName(s), () =>
        {
            _levelUp.LevelUp(party, _hero, s);
            FinishLevelUpIfDone();
        });

    private void FinishLevelUpIfDone()
    {
        if (_hero.LevelUpPoints < 1)
            Message.Publish(new Finished<LevelUpHero>());
    }
}
