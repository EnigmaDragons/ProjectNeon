using System;
using System.Collections.Generic;

public static class AllCutsceneSegments
{
    private static readonly Dictionary<CutsceneSegmentType, Func<CutsceneSegmentData, CutsceneSegment>>
        CreateSegmentOfType = new Dictionary<CutsceneSegmentType, Func<CutsceneSegmentData, CutsceneSegment>>
        {
            {CutsceneSegmentType.Nothing, e => new SimpleSegment(() => { })},
            {CutsceneSegmentType.DialogueLine, e => new MessagePublishSegment(
                new ShowCharacterDialogueLine(e.DialogueCharacterId, e.Text), 
                new FullyDisplayDialogueLine(e.DialogueCharacterId))},
            {CutsceneSegmentType.NarratorLine, e => new MessagePublishSegment(
                new ShowCharacterDialogueLine(CutsceneCharacterAliases.Narrator, e.Text),
                new FullyDisplayDialogueLine(CutsceneCharacterAliases.Narrator))}
        };

    public static CutsceneSegment Create(CutsceneSegmentData data)
    {
        var type = data.SegmentType;
        try
        {
            if (!CreateSegmentOfType.ContainsKey(type))
            {
                Log.Error($"No SegmentType of {type} exists in {nameof(AllCutsceneSegments)}");
#if UNITY_EDITOR
                throw new KeyNotFoundException($"Construction Details not found for Segment Type {type}");
#endif
                return CreateSegmentOfType[CutsceneSegmentType.Nothing](data);
            }

            return CreateSegmentOfType[type](data);
        }
        catch (Exception e)
        {
            Log.Error($"SegmentType {type} is broken {e}");
#if UNITY_EDITOR
            throw;
#endif
            return CreateSegmentOfType[CutsceneSegmentType.Nothing](data);
        }
    }
}
