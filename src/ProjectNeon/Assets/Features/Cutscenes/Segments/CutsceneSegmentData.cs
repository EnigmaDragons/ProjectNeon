using System;
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
}
