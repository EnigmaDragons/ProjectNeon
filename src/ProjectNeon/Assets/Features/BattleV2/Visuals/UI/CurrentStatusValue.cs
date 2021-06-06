using System;
using UnityEngine;

[Serializable]
public sealed class CurrentStatusValue
{
    public string Type = "";
    public Sprite Icon;
    public string Text = "";
    public string Tooltip = "";
    public bool IsChanged = false;
    public Maybe<int> OriginatorId = Maybe<int>.Missing();

    public bool IsSameTypeAs(CurrentStatusValue other) => other.Type.Equals(Type);
    public bool IsChangedFrom(CurrentStatusValue other) => IsSameTypeAs(other) && !other.Text.Equals(Text);
}
