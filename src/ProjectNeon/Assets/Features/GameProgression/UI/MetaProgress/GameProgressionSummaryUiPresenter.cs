using System;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;

public class GameProgressionSummaryUiPresenter : MonoBehaviour
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField] private TextMeshProUGUI totalPercentLabel;
    [SerializeField] private Localize listSummary;
    
    private void OnEnable()
    {
        Render();
    }

    private void Render()
    {
        var progressItems = progress.GetAllProgress();
        var percentage = progressItems.Count(x => x.Completed) / (float) progressItems.Length;
        totalPercentLabel.text = $"{percentage:P}";
        listSummary.SetFinalText(string.Join(Environment.NewLine, progressItems.Select(p => p.FullDescription())));
    }
}
