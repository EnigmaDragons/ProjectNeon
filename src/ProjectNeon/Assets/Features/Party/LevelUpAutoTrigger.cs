using System.Linq;
using UnityEngine;

public class LevelUpAutoTrigger : OnMessage<PartyAdventureStateChanged, HeroLevelledUp>
{
    [SerializeField] private PartyAdventureState party;

    private void Start() => TriggerNextLevelUpIfApplicable();

    protected override void Execute(PartyAdventureStateChanged msg) => TriggerNextLevelUpIfApplicable();
    protected override void Execute(HeroLevelledUp msg) => TriggerNextLevelUpIfApplicable();

    private void TriggerNextLevelUpIfApplicable()
    {
        foreach (var hero in party.Heroes.Where(h => h != null))
            if (hero.Levels.UnspentLevelUpPoints > 0)
            {
                Log.Info($"Hero is null {hero == null}. Hero Name is Null {hero.NameTerm == null}. Hero Character is Null {hero.Character == null}");
                Log.Info($"{hero.NameTerm.ToEnglish()} - XP {hero.Levels.Xp} - Unspent Points {hero.Levels.UnspentLevelUpPoints}");
                Message.Publish(new LevelUpHero(hero));
                return;
            }
    }
}