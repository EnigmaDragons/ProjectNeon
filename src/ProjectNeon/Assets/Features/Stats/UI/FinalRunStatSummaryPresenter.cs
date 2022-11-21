using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalRunStatSummaryPresenter : MonoBehaviour, ILocalizeTerms
{
    [SerializeField] private AdventureConclusionState state;
    [SerializeField] private Localize[] localizedLabels;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI[] values;
    [SerializeField] private GameObject[] heroObjects;
    [SerializeField] private Image[] heroBusts;

    private void OnEnable() => Render();

    private void Render()
    {
        var stats = state.Stats;
        var timeSpan = TimeSpan.FromSeconds(stats.TimeElapsedSeconds.CeilingInt());
        var timeText = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        RenderHeroes();
        Render(0, "Conclusions/Run Duration", timeText);
        Render(1, "Conclusions/Cards Played", stats.TotalCardsPlayed);
        Render(2, "Conclusions/Turns Played", stats.TotalTurnsPlayed);
        Render(3, "Conclusions/Enemies Defeated", stats.TotalEnemiesKilled);
        Render(4, "Conclusions/Damage Dealt", stats.TotalDamageDealt);
        Render(5, "Conclusions/Damage Taken", stats.TotalDamageReceived);
        Render(6, "Conclusions/HP Damage Taken", stats.TotalHpDamageReceived);
        Render(7, "Conclusions/Healing Received", stats.TotalHealingReceived);
        Hide(8);
    }

    private void RenderHeroes()
    {
        for (var i = 0; i < heroObjects.Length; i++)
        {
            heroObjects[i].SetActive(false);
            try
            {
                var heroActive = state.Heroes.Length > i;
                if (heroActive)
                {
                    heroBusts[i].sprite = state.Heroes[i].Bust;
                    heroObjects[i].SetActive(true);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }

    private void Hide(int index)
    {
        if (localizedLabels.Length < index || values.Length < index)
            return;
        
        localizedLabels[index].SetFinalText("");
        values[index].text = "";
    }
    
    private void Render(int index, string term, int value)
        => Render(index, term, value.ToString());
    private void Render(int index, string term, string value)
    {
        if (localizedLabels.Length < index || values.Length < index)
            return;
        
        localizedLabels[index].SetTerm(term);
        values[index].text = value;
    }

    public string[] GetLocalizeTerms()
        => new[]
        {
            "Conclusions/Run Duration",
            "Conclusions/Cards Played",
            "Conclusions/Turns Played",
            "Conclusions/Enemies Defeated",
            "Conclusions/Damage Dealt",
            "Conclusions/Damage Taken",
            "Conclusions/HP Damage Taken",
            "Conclusions/Healing Received"
        };
}
