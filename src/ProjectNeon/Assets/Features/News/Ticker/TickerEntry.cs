using System;
using UnityEngine;

[Serializable]
public class TickerEntry
{
    [TextArea(1, 2), SerializeField] private string text;

    public string Text => text;
}
