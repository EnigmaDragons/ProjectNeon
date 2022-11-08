using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

[Serializable]
public class CutsceneSegmentData
{
    public int Id;
    public int AdventureId;
    public CutsceneSegmentType SegmentType;
    public StringReference DialogueCharacterId = new StringReference { UseConstant = false };
    [TextArea(4, 4)] public string Text = "";
    
    public FloatReference FloatAmount = new FloatReference(0);
    public StoryEvent2 StoryEvent;
    public StringReference[] RequiredStates;
    public StringReference[] ForbiddenStates;
    public bool Or;
    public StringReference StoryState = new StringReference("");
    public string Term => AdventureId > 0 ? $"Cutscenes/Adventure{AdventureId}_Segment{Id}" : $"Cutscenes/Orphaned_Segment{Id}";

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
    
    public string[] GetExportDescription()
    {
        if (SegmentType == CutsceneSegmentType.Nothing)
            return new [] { "Nothing" };
        if (SegmentType == CutsceneSegmentType.NarratorLine)
            return new [] { $"Narrator: \"{Text}\"" };
        if (SegmentType == CutsceneSegmentType.DialogueLine)
            return new [] { $"{DialogueCharacterId.Value}: \"{Text}\"" };
        if (SegmentType == CutsceneSegmentType.ShowCharacter)
            return new [] { $"Enter {DialogueCharacterId.Value}" };
        if (SegmentType == CutsceneSegmentType.HideCharacter)
            return new [] { $"Exit {DialogueCharacterId.Value}" };
        if (SegmentType == CutsceneSegmentType.Choice)
            return GetChoiceDescription();
        if (SegmentType == CutsceneSegmentType.MultiChoice)
            return GetChoiceDescription();
        if (SegmentType == CutsceneSegmentType.RecordStoryState)
            return new [] { $"Story State:{StoryState.Value} - true" };
        if (SegmentType == CutsceneSegmentType.PlayerLine)
            return new [] { $"Player: \"{Text}\"" };
        return new [] { "Unknown Cutscene Segment" };
    }

    private string[] GetChoiceDescription()
    {
        var lines = new List<string>();
        lines.Add($"Present Player Game With {(StoryEvent.IsMultiChoice ? "Multi-Choice" : "Choice")}: {StoryEvent.DisplayName}");
        lines.Add($"  Choice Prompt: \"{StoryEvent.Term.ToEnglish()}\"");
        for (var i = 0; i < StoryEvent.Choices.Length; i++)
        {
            var choice = StoryEvent.Choices[i];
            if (choice.Resolution.Length == 1 && choice.Resolution[0].Result is RecordChoiceResult)
                lines.Add($"    {i + 1}. \"{choice.Term.ToEnglish()}\" - {((RecordChoiceResult)choice.Resolution[0].Result).Name} - {((RecordChoiceResult)choice.Resolution[0].Result).state}");
            else
                lines.Add($"    {i + 1}. \"{choice.Term.ToEnglish()}\"");
        }
        return lines.ToArray();
    }
}
