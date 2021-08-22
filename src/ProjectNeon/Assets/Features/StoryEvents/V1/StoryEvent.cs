using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/StoryEvent")]
public class StoryEvent : ScriptableObject
{
    [SerializeField] private StorySetting settingType;
    [SerializeField, TextArea(4, 10)] private string storyText;
    [SerializeField] private StoryEventChoice[] choices;

    public StorySetting StorySetting => settingType;
    public string StoryText => storyText;
    public StoryEventChoice[] Choices => choices.ToArray();
}
