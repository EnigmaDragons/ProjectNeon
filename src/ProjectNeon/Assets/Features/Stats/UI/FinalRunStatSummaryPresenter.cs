using System;
using TMPro;
using UnityEngine;

public class FinalRunStatSummaryPresenter : MonoBehaviour
{
    [SerializeField] private AdventureConclusionState state;
    [SerializeField] private TextMeshProUGUI[] labels;
    [SerializeField] private TextMeshProUGUI[] values;

    private void OnEnable() => Render();

    private void Render()
    {
        var stats = state.Stats;
        var timeSpan = TimeSpan.FromSeconds(stats.TimeElapsedSeconds.CeilingInt());
        var timeText = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        Render(0, "Run Duration", timeText);
        Render(1, "Cards Played", stats.TotalCardsPlayed);
        Render(2, "Turns Played", stats.TotalTurnsPlayed);
        Render(3, "Enemies Defeated", stats.TotalEnemiesKilled);
        Render(4, "Damage Dealt", stats.TotalDamageDealt);
        Render(5, "Damage Taken", stats.TotalDamageReceived);
        Render(6, "HP Damage Taken", stats.TotalHpDamageReceived);
        Render(7, "Healing Received", stats.TotalHealingReceived);
    }

    private void Render(int index, string label, int value)
        => Render(index, label, value.ToString());
    private void Render(int index, string label, string value)
    {
        if (labels.Length < index || values.Length < index)
            return;
        
        labels[index].text = label;
        values[index].text = value;
    }
}
