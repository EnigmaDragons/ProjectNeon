using System.Linq;
using UnityEngine;

public class StartTutorialBattleButton : MonoBehaviour
{
    [SerializeField] private BattleState battleState;
    [SerializeField] private LocalizedCommandButton button;
    [SerializeField] private Navigator navigator;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BaseHero tutorialHero;
    [SerializeField] private CurrentCutscene current;
    
    public void Init(string term, SpecificEncounterSegment tutorial)
    {
        button.InitTerm(term, () =>
        {
            battleState.IsSingleTutorialBattle = true;
            party.Initialized(tutorialHero, null, null);
            battleState.SetNextBattleground(tutorial.battlefield);
            battleState.SetNextEncounter(tutorial.enemies.Select(x => x.ForStage(1)).ToArray(), tutorial.isElite, false, true, tutorial.overrideDeck);
            if (tutorial.shouldOverrideStartingCards) 
                battleState.SetNextBattleStartingCardCount(tutorial.overrideStartingCardsCount);
            if (!tutorial.allowBasic) 
                battleState.SetAllowSwapToBasic(tutorial.allowBasic);
            if (!tutorial.allowCycleOrDiscard) 
                battleState.SetAllowCycleOrDiscard(tutorial.allowCycleOrDiscard);
            if (tutorial.Cutscene != null)
                current.InitStartBattle(tutorial.Cutscene);
            navigator.NavigateToBattleScene();
        });
    }
}