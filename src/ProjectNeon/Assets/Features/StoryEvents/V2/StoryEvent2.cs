using System.Linq;
using System.Text;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/StoryEvent2")]
public class StoryEvent2 : ScriptableObject, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField] public string storyText;
    [SerializeField] private StorySetting settingType;
    [SerializeField] private StaticCorp corp;
    [SerializeField] private StoryEventChoice2[] choices;
    [SerializeField] private bool inCutscene;
    [SerializeField] private bool isMultiChoice;

    public string DisplayName => new LocalizedString($"Event{id}");
    
    public StorySetting StorySetting => settingType;
    public StaticCorp Corp => corp;
    public string Term => $"StoryEvents/Event{id}";
    public StoryEventChoice2[] Choices => choices.ToArray();
    public bool InCutscene => inCutscene;
    public bool IsMultiChoice => isMultiChoice;

    public override string ToString() => ToString(this);

    private static string ToString(StoryEvent2 s)
    {
        var sb = new StringBuilder();
        sb.Append(s.Term.ToEnglish());
        sb.AppendLine();
        foreach (var choice in s.Choices)
        {
            sb.AppendLine($"Choice: {choice.Term.ToEnglish()}");
            foreach (var resolution in choice.Resolution)
                if (!resolution.HasContinuation)
                    sb.AppendLine($"Outcome: {resolution.Result}");
                else
                    sb.AppendLine(ToString(resolution.ContinueWith));
        }

        return sb.ToString();
    }

    public string[] GetLocalizeTerms()
        => choices.Select(x => x.Term).Concat(Term).ToArray();
}