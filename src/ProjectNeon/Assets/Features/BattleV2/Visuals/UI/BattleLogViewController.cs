using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[IgnoreForLocalization]
public sealed class BattleLogViewController : OnMessage<WriteBattleLogMessageRequested, ToggleBattleLogView>
{
    [SerializeField] private BoolReference loggingEnabled;
    [SerializeField] private TextMeshProUGUI textArea;
    [SerializeField] private GameObject view;
    [SerializeField] private Scrollbar scroll;
    [SerializeField] private int maxLines = 40;
    
    private readonly List<string> _battleLogMessages = new List<string>();

    private void Awake() => textArea.text = string.Empty;
    
    protected override void Execute(WriteBattleLogMessageRequested msg)
    {
        if (!loggingEnabled)
            return;
        
        _battleLogMessages.Add(msg.Message);
        textArea.text = string.Join("\n", _battleLogMessages);
    }

    protected override void Execute(ToggleBattleLogView msg)
    {
        if (!loggingEnabled)
            return;

        view.SetActive(!view.activeSelf);
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return new WaitForSeconds(0.2f);
        scroll.value = 0;
    }
}