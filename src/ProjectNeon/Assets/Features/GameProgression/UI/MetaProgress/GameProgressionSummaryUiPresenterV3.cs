using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameProgressionSummaryUiPresenterV3 : MonoBehaviour
{
    [SerializeField] private ProgressionProgress progress;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI totalPercentLabel;
    [SerializeField] private DifficultyItemCompletionPresenter[] heroPresenters;
    [SerializeField] private DifficultyItemCompletionPresenter[] bossPresenters;
    [SerializeField] private DifficultyItemCompletionPresenter[] adventurePresenters;
    [SerializeField] private DifficultyItemCompletionPresenter[] difficultyPresenters;
    
    private void OnEnable()
    {
        Render();
    }

    private void Render()
    {
        var progressItems = progress.GetAllProgress();
        var percentage = progressItems.Count(x => x.Completed) / (float) progressItems.Length;
        totalPercentLabel.text = $"{percentage:P}";
        
        InitPresenters(heroPresenters, progressItems.Where(x => x.Hero.IsPresent).ToArray(), x => x.Hero.Value.Bust);
        InitPresenters(bossPresenters, progressItems.Where(x => x.Boss.IsPresent).ToArray(), x => x.Boss.Value.Bust);
        InitPresenters(adventurePresenters, progressItems.Where(x => x.Adventure.IsPresent).ToArray(), x => x.Adventure.Value.AdventureImage);
        InitPresenters(difficultyPresenters, progressItems.Where(x => x.IsDifficultyOnly).ToArray(), x => x.Difficulty.Value.Image);
    }

    private void InitPresenters(DifficultyItemCompletionPresenter[] presenters, ProgressionItem[] items, Func<ProgressionItem, Sprite> getArt)
    {
        for (var i = 0; i < presenters.Length; i++)
        {
            var p = presenters[i];
            var shouldShow = items.Length > i;
            if (!shouldShow)
            {
                p.Hide();
                continue;
            }

            var item = items[i];
            Log.Info(item.FullDescription());
            Log.Info($"Difficulty Is Present - {item.Difficulty.IsPresent}");
            p.Init(getArt(item), item.Difficulty, item.Completed);
        }
    }
}
