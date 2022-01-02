using TMPro;
using UnityEngine;

public class BattleStatSummaryPresenter : MonoBehaviour
{
    [SerializeField] private BattleState state;
    [SerializeField] private TextMeshProUGUI[] labels;
    [SerializeField] private TextMeshProUGUI[] values;

    private void OnEnable() => Render();

    private void Render()
    {
        var stats = state.Stats;
        Render(0, "Cards Played", stats.CardsPlayed);
        Render(1, "Damage Dealt", stats.DamageDealt);
        Render(2, "Damage Taken", stats.DamageReceived);
        Render(3, "HP Damage Taken", stats.HpDamageReceived);
        Render(4, "Healing Received", stats.HealingReceived);
    }

    private void Render(int index, string label, int value)
    {
        if (labels.Length < index || values.Length < index)
            return;
        
        labels[index].text = label;
        values[index].text = value.ToString();
    }
}
