using TMPro;
using UnityEngine;

public class FinalRunStatSummaryPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] labels;
    [SerializeField] private TextMeshProUGUI[] values;

    private void OnEnable() => Render();

    private void Render()
    {
        var stats = CurrentGameData.Data.Stats;
        Render(0, "Run Duration", $"{(stats.TimeElapsedSeconds / 60):F0}:{stats.TimeElapsedSeconds % 60}");
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
