using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(menuName = "StoryEvent/StoryEvent2")]
public class StoryEvent2 : ScriptableObject
{
    [SerializeField, ReadOnly] public int id;
    [SerializeField] private StorySetting settingType;
    [SerializeField] private StaticCorp corp;
    [SerializeField] private StoryEventChoice2[] choices;
    [SerializeField] private bool inCutscene;
    [SerializeField] private bool isMultiChoice;

    public string DisplayName => Localize.GetEvent($"Event{id}");
    
    public StorySetting StorySetting => settingType;
    public StaticCorp Corp => corp;
    public string StoryText => Localize.GetEvent($"Event{id} Story");
    public StoryEventChoice2[] Choices => choices.ToArray();
    public bool InCutscene => inCutscene;
    public bool IsMultiChoice => isMultiChoice;
}