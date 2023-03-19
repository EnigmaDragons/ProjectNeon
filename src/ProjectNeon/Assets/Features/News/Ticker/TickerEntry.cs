using System;
using UnityEngine;

[Serializable]
public class TickerEntry
{
    [SerializeField] private string term;

    public string Term => $"News/{term}";
}
