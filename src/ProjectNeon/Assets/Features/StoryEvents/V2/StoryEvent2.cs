using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/StoryEvent2")]
public class StoryEvent2 : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private StorySetting settingType;
    [SerializeField] private StaticCorp corp;
    [SerializeField, TextArea(4, 10)] private string storyText;
    [SerializeField] private StoryEventChoice2[] choices;

    public string DisplayName => string.IsNullOrWhiteSpace(displayName) 
        ? name.WithSpaceBetweenWords() 
        : displayName;
    
    public StorySetting StorySetting => settingType;
    public StaticCorp Corp => corp;
    public string StoryText => storyText;
    public StoryEventChoice2[] Choices => choices.ToArray();
}