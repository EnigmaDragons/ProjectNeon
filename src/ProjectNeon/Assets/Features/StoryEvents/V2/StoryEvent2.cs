using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "StoryEvent/StoryEvent2")]
public class StoryEvent2 : ScriptableObject
{
    [SerializeField, ReadOnly] public int id;
    [SerializeField] private StorySetting settingType;
    [SerializeField] private StaticCorp corp;
    [SerializeField] private StoryEventChoice2[] choices;

    public string DisplayName => LocalizationSettings.StringDatabase.GetLocalizedString("Events", $"Event{id}");
    
    public StorySetting StorySetting => settingType;
    public StaticCorp Corp => corp;
    public string StoryText => LocalizationSettings.StringDatabase.GetLocalizedString("Events", $"Event{id} Story");
    public StoryEventChoice2[] Choices => choices.ToArray();
}