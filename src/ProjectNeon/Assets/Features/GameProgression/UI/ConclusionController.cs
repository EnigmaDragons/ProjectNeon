using UnityEngine;

public class ConclusionController : MonoBehaviour
{
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private AdventureConclusionState conclusion;
    [SerializeField] private ConclusionPresenter presenter;

    private void Start()
    {
        presenter.Init(conclusion.IsVictorious, adventure.Adventure.Title, conclusion.EndingStoryText);
    }
}
