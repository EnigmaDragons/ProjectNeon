using System;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;

public class GameProgressionSummaryUiPresenterV2 : MonoBehaviour
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI totalPercentLabel;
    [SerializeField] private CompletionProgressPresenter[] completionPresenters;
    [SerializeField] private Localize nonVisualCompletionItemsLabel;
    
    private void OnEnable()
    {
        Render();
    }

    private void Render()
    {
        var progressItems = progress.GetAllProgress();
        var percentage = progressItems.Count(x => x.Completed) / (float) progressItems.Length;
        totalPercentLabel.text = $"{percentage:P}";

        var groupedProgress = progressItems.GroupBy(p => p.Adventure.Select(a => a.Id, -1));
        var progressByAdventure = groupedProgress.Where(x => x.Key != -1).ToArray();
        for (var i = 0; i < completionPresenters.Length; i++)
        {
            var shouldShow = progressByAdventure.Length > i;
            if (!shouldShow)
            {
                completionPresenters[i].Hide();
                continue;
            }

            var progressItem = progressByAdventure[i];
            completionPresenters[i].Initialized(progressItem.First().Adventure.Value, progressItem.ToArray());
        }
        
        nonVisualCompletionItemsLabel.SetFinalText(string.Join(Environment.NewLine, groupedProgress
            .Where(x => x.Key == -1)
            .SelectMany(x => x.ToArray())
            .Select(p => p.FullDescription())));
    }
}
