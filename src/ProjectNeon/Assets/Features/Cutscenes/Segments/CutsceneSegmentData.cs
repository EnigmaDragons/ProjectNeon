using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CutsceneSegmentData
{
    public CutsceneSegmentType SegmentType;
    public StringReference DialogueCharacterId = new StringReference { UseConstant = false };
    [TextArea(4, 4)] public string Text = "";
    public FloatReference FloatAmount = new FloatReference(0);
    public StoryEvent2 StoryEvent;
    public StringReference[] RequiredStates;
    public StringReference[] ForbiddenStates;
    public bool Or;
    public StringReference StoryState;

    public bool ShouldShow(Func<string, bool> storyState)
        => !ShouldSkip(storyState);
    
    public bool ShouldSkip(Func<string, bool> storyState)
        => ForbiddenStates.Any(x => storyState(x))
           || (Or && RequiredStates.None(x => storyState(x)))
           || (!Or && RequiredStates.Any(x => !storyState(x)));

    public Maybe<string> GetRequiredConditionsDescription()
    {
        if (RequiredStates.None() && ForbiddenStates.None())
            return Maybe<string>.Missing();
        
        var conditions = "";
        if (RequiredStates.Any())
            conditions += $"Required: {string.Join(", ", RequiredStates.Select(r => r.Value))}. ";
        if (ForbiddenStates.Any())
            conditions += $"Skipped If: {string.Join(", ", ForbiddenStates.Select(r => r.Value))}. ";
        return Maybe<string>.Present(conditions.Trim());
    }
    
    public string GetExportDescription()
    {
        if (SegmentType == CutsceneSegmentType.Nothing)
            return "Nothing";
        if (SegmentType == CutsceneSegmentType.NarratorLine)
            return $"Narrator: \"{Text}\"";
        if (SegmentType == CutsceneSegmentType.DialogueLine)
            return $"{DialogueCharacterId.Value}: \"{Text}\"";
        if (SegmentType == CutsceneSegmentType.ShowCharacter)
            return $"Enter {DialogueCharacterId.Value}";
        if (SegmentType == CutsceneSegmentType.HideCharacter)
            return $"Exit {DialogueCharacterId.Value}";
        if (SegmentType == CutsceneSegmentType.Choice)
            return $"-- Present Player Game Choice --";
        if (SegmentType == CutsceneSegmentType.MultiChoice)
            return $"-- Present Player Game Multi-Choice --";
        if (SegmentType == CutsceneSegmentType.RecordStoryState)
            return $"-- Save Player Story Choice --";
        return "Unknown Cutscene Segment";
    }
}
