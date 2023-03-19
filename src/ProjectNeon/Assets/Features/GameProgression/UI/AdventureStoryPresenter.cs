using I2.Loc;
using UnityEngine;

public class AdventureStoryPresenter : MonoBehaviour
{
    [SerializeField] private Localize title;
    [SerializeField] private Localize story;
    [SerializeField] private CurrentAdventure current;

    private void Awake()
    {
        title.SetTerm(current.Adventure.TitleTerm);
        story.SetTerm(current.Adventure.StoryTerm);
    }
}
