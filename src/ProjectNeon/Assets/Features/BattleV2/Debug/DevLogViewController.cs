using System.Collections.Generic;
using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public sealed class DevLogViewController : OnMessage<WriteDevLogMessageRequested, ToggleDevLogView>
{
    [SerializeField] private BoolReference loggingEnabled;
    [SerializeField] private TextMeshProUGUI textArea;
    [SerializeField] private GameObject view;
    [SerializeField] private int maxLines = 40;
    
    private readonly List<string> _battleLogMessages = new List<string>();
    
    protected override void Execute(WriteDevLogMessageRequested msg)
    {
        if (!loggingEnabled)
            return;
        
        _battleLogMessages.Add(msg.Message);
        while (_battleLogMessages.Count > maxLines)
            _battleLogMessages.RemoveAt(0);
        textArea.text = string.Join("\n", _battleLogMessages);
    }
    
    protected override void Execute(ToggleDevLogView msg)
    {
        if (!loggingEnabled)
            return;

        view.SetActive(!view.activeSelf);
    }
}
