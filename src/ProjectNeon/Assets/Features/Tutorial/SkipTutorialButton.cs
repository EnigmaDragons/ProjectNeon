using UnityEngine;

public class SkipTutorialButton : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private LocalizedCommandButton button;

    private void Start()
    {
        if (adventure.Adventure.Id != AdventureIds.TutorialAdventureId)
            button.gameObject.SetActive(false);
        else if (CurrentAcademyData.Data.IsLicensedBenefactor)
            button.InitTerm("Tutorials/SkipTutorial", () => TutorialWonHandler.Execute(0f));
        else 
            button.InitTerm("Tutorials/SkipTutorial", () => Message.Publish(new ShowLocalizedDialog
            {
                PrimaryAction = () => TutorialWonHandler.Execute(0f),
                PrimaryButtonTerm = DialogTerms.OptionSkipTutorial,
                SecondaryAction = () => {},
                SecondaryButtonTerm = DialogTerms.OptionOops,
                PromptTerm = DialogTerms.SkipTutorialWarning
            }));
    }

    public string[] GetLocalizeTerms() => new []{DialogTerms.SkipTutorialWarning, DialogTerms.OptionSkipTutorial, DialogTerms.OptionOops};
}