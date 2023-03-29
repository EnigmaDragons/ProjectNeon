using System.Linq;
using UnityEngine;

public class TutorialBattlesUI : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private GameObject panel;
    [SerializeField] private StartTutorialBattleButton tutorialButtonPrototype;
    [SerializeField] private TermedTuturial[] tutorials;

    private void Start()
    {
        foreach (var tutorial in tutorials)
            Instantiate(tutorialButtonPrototype, panel.transform).Init(GetTerm(tutorial), tutorial.Tutorial);
    }

    private string GetTerm(TermedTuturial tutorial) => $"Tutorials/{tutorial.Term}";

    public string[] GetLocalizeTerms() => tutorials.Select(GetTerm).ToArray();
}