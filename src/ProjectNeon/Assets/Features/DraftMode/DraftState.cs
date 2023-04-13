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
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickGear,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.PickCard,
        DraftStep.Finished
    };

    public bool Init(int numHeroes, DraftData data)
    {
        numDraftHeroes = numHeroes;
        heroIndex = data.HeroIndex;
        draftStepIndex = data.DraftStepIndex;
        return true;
    }

    public int HeroIndex => heroIndex;

    public bool PickedHeroes() => draftStepIndex > DraftSteps.LastIndexOf(x => x == DraftStep.PickHero);
    
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
        Message.Publish(new DraftStateUpdated(draftStepIndex + 1, DraftSteps.Length - 1, heroIndex));
        return DraftSteps[draftStepIndex];
    }
    
    public DraftStep Current => DraftSteps[draftStepIndex];

    public DraftData GetData() => new DraftData {HeroIndex = heroIndex, DraftStepIndex = draftStepIndex};
}
