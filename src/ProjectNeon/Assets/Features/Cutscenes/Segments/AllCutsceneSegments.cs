using System;
using System.Collections.Generic;
using UnityEngine;

public enum CutsceneSegmentType
{
    Nothing = 0,
    DialogueLine = 1,
}

[Serializable]
public class CutsceneSegmentData
{
    public CutsceneSegmentType SegmentType;
    public StringReference DialogueCharacterId = new StringReference { UseConstant = false };
    [TextArea(4, 4)] public string Text = "";
}

public static class AllCutsceneSegments
{
    private static readonly Dictionary<CutsceneSegmentType, Func<CutsceneSegmentData, CutsceneSegment>>
        CreateSegmentOfType = new Dictionary<CutsceneSegmentType, Func<CutsceneSegmentData, CutsceneSegment>>
        {
            {CutsceneSegmentType.Nothing, e => new SimpleSegment(() => { })},
            {CutsceneSegmentType.DialogueLine, e => new SimpleSegment(
                () => Message.Publish(new ShowCharacterDialogueLine(e.DialogueCharacterId, e.Text)), 
                () => Message.Publish(new FullyDisplayDialogueLine(e.DialogueCharacterId)))},
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
