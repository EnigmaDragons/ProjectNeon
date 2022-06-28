using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/DraftState")]
public class DraftState : ScriptableObject
{
    [SerializeField] private int numDraftHeroes = 0;
    [SerializeField] private int heroIndex = -1;
    [SerializeField] private int draftStepIndex = 0;
    
    private DraftStep[] DraftSteps = {
        DraftStep.PickHero,
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.Finished
    };

    public void Init(int numHeroes)
    {
        numDraftHeroes = numHeroes;
        heroIndex = -1;
        draftStepIndex = 0;
    }

    public int HeroIndex => heroIndex;

    public DraftStep Advance()
    {
        Debug.Log("Draft - Advance Hero");
        heroIndex++;
        if (heroIndex >= numDraftHeroes)
        {
            Debug.Log("Draft - Advance Draft Step");
            heroIndex = 0;
            draftStepIndex++;
        }
        if (draftStepIndex >= DraftSteps.Length)
        {
            return DraftStep.Finished;
        }
        Debug.Log($"Draft - Advance - Hero {heroIndex} - Draft Step {draftStepIndex}");
        return DraftSteps[draftStepIndex];
    }
}
