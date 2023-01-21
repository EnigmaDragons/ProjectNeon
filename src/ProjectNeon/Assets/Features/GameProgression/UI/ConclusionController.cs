using UnityEngine;

public class ConclusionController : MonoBehaviour
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private ConclusionPresenter presenter;

    private void Start()
    {
        Log.Info($"Init Conclusion Presenter - {conclusion.IsVictorious} - {conclusion.EndingStoryTextTerm}");
        presenter.Init(conclusion.IsVictorious, adventure.Adventure.TitleTerm, conclusion.EndingStoryTextTerm);
    }
}
