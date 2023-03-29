using UnityEngine;

public class SkipTutorialBattleButton : OnMessage<WinBattleWithRewards>, ILocalizeTerms
{
    [SerializeField] private LocalizedCommandButton button;
    [SerializeField] private BattleState battleState;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private EventPublisher eventPublisher;

    private string Term => "Tutorials/SkipTutorial";
    
    private void Start()
    {
        button.gameObject.SetActive(adventure.Adventure.Id == AdventureIds.TutorialAdventureId || battleState.IsSingleTutorialBattle);
        if (battleState.IsSingleTutorialBattle)
            button.InitTerm(Term, () => eventPublisher.WinBattle());
        else if (adventure.Adventure.Id == AdventureIds.TutorialAdventureId)
            button.InitTerm(Term, () => Message.Publish(new ShowLocalizedDialog
            {
                PrimaryAction = () => eventPublisher.WinBattle(),
                PrimaryButtonTerm = DialogTerms.OptionSkipBattle,
                SecondaryAction = () => {},
                SecondaryButtonTerm = DialogTerms.OptionOops,
                PromptTerm = DialogTerms.SkipTutorialBattleWarning
            }));
    }

    public string[] GetLocalizeTerms()
        => new[] {Term, DialogTerms.OptionSkipBattle, DialogTerms.OptionOops, DialogTerms.SkipTutorialBattleWarning};

    protected override void Execute(WinBattleWithRewards msg)
        => button.gameObject.SetActive(false);
}