using I2.Loc;
using TMPro;
using UnityEngine;

public class BattleStatSummaryPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private BattleState state;
    [SerializeField] private Localize[] localizedLabels;
    [SerializeField] private TextMeshProUGUI[] values;

    private void OnEnable() => Render();

    private void Render()
    {
        var stats = state.Stats;
        Render(0, "Conclusions/Cards Played", stats.CardsPlayed);
        Render(1, "Conclusions/Damage Dealt", stats.DamageDealt);
        Render(2, "Conclusions/Damage Taken", stats.DamageReceived);
        Render(3, "Conclusions/HP Damage Taken", stats.HpDamageReceived);
        Render(4, "Conclusions/Healing Received", stats.HealingReceived);
    }

    private void Render(int index, string term, int value)
    {
        if (localizedLabels.Length < index || values.Length < index)
            return;
        
        localizedLabels[index].SetTerm(term);
        values[index].text = value.ToString();
    }

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Conclusions/Cards Played",
            "Conclusions/Damage Dealt",
            "Conclusions/Damage Taken",
            "Conclusions/HP Damage Taken",
            "Conclusions/Healing Received"
        };
}
