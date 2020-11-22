
using TMPro;
using UnityEngine;

public class AdventureStoryPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI story;
    [SerializeField] private CurrentAdventure current;

    private void Awake()
    {
        title.text = current.Adventure.Title;
        story.text = current.Adventure.Story;
    }
}
