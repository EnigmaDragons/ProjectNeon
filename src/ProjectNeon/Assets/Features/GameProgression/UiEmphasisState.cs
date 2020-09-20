using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Only Once/UiEmphasisState")]
public class UiEmphasisState : ScriptableObject
{
    [SerializeField] private List<string> elements = new List<string>();

    public bool Contains(string elementName) => elements.Contains(elementName);
    public void Add(string elementName) => UpdateState(() => elements.Add(elementName));

    public void Remove(string elementName)
    {
        var numRemoved = elements.RemoveAll(x => x.Equals(elementName));
        if (numRemoved > 0)
            Message.Publish(new UiEmphasisStateChanged());
    }

    private void UpdateState(Action a)
    {
        a();
        Message.Publish(new UiEmphasisStateChanged());
    }
}
