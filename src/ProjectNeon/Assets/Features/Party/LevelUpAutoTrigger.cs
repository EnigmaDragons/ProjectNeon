using UnityEngine;

public class LevelUpAutoTrigger : OnMessage<PartyAdventureStateChanged, HeroLevelledUp>
{
    [SerializeField] private PartyAdventureState party;

    private void Start() => TriggerNextLevelUpIfApplicable();

    protected override void Execute(PartyAdventureStateChanged msg) => TriggerNextLevelUpIfApplicable();
    protected override void Execute(HeroLevelledUp msg) => TriggerNextLevelUpIfApplicable();

    private void TriggerNextLevelUpIfApplicable()
    {
        foreach (var hero in party.Heroes)
            if (hero.Levels.UnspentLevelUpPoints > 0)
            {
                Message.Publish(new LevelUpHero(hero));
                return;
            }
    }
}