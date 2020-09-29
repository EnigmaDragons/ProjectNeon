
using TMPro;
using UnityEngine;

public class AdventureStoryPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI story;
    [SerializeField] private AdventureProgress adventure;

    private void Awake()
    {
        title.text = adventure.Adventure.Title;
        story.text = adventure.Adventure.Story;
    }
}
