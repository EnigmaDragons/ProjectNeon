using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompletionProgressPresenter : MonoBehaviour
{
    [SerializeField] private Image adventureCover;
    [SerializeField] private TextMeshProUGUI adventureTitle;
    [SerializeField] private HeroCompletionPresenter[] heroes;
    [SerializeField] private GameObject lockedObj;
    [SerializeField] private TextMeshProUGUI lockReasonLabel;
    [SerializeField] private GameObject completionCheckmark;

    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private Localize highestCompletedDifficultyLabel;
    [SerializeField] private GameObject difficultiesCompletionCheckmark;

    public CompletionProgressPresenter Initialized(Adventure adv, ProgressionItem[] progressItems)
        => Initialized(adv.MapTitle, adv.AdventureImage, progressItems, adv.IsLocked ? adv.LockConditionExplanation : Maybe<string>.Missing());
    
    public CompletionProgressPresenter Initialized(string title, Sprite coverArt, ProgressionItem[] progressItems, Maybe<string> lockReason)
    {
        adventureCover.sprite = coverArt;
        adventureTitle.text = title;
        RenderHeroes(progressItems);
        completionCheckmark.SetActive(progressItems.Where(p => p.Difficulty.IsMissing).All(p => p.Completed));
        RenderDifficulty(progressItems);
        lockedObj.SetActive(lockReason.IsPresent);
        if (lockReason.IsPresent)
            lockReasonLabel.text = lockReason.Value;
        gameObject.SetActive(true);
        return this; 
    }

    private void RenderDifficulty(ProgressionItem[] progressItems)
    {
        var completedAnyDifficulties = progressItems.Any(p => p.Completed && p.Difficulty.IsPresent);
        if (!completedAnyDifficulties)
        {
            if (difficultyPanel != null)
                difficultyPanel.SetActive(false);
            return;
        }
        
        difficultyPanel.SetActive(true);
        if (difficultiesCompletionCheckmark != null)
            difficultiesCompletionCheckmark.SetActive(
                progressItems.Where(p => p.Difficulty.IsPresent).All(p => p.Completed));
        if (highestCompletedDifficultyLabel != null)
            highestCompletedDifficultyLabel.SetTerm(progressItems.Where(p => p.Completed && p.Difficulty.IsPresent)
                .OrderByDescending(p => p.Difficulty.Select(d => d.id, -2)).First().Difficulty.Value.NameTerm);
    }

    public void Hide() => gameObject.SetActive(false);
    
    private void RenderHeroes(ProgressionItem[] progressItems)
    {
        for (var i = 0; i < heroes.Length; i++)
        {
            var o = heroes[i];
            if (progressItems.Length <= i)
            {
                o.Hidden();
                continue;
            }

            var p = progressItems[i];
            if (p.Hero.IsMissing)
            {
                o.Hidden();
                continue;
            }

            o.Initialized(p.Hero.Value, p.Completed);
        }
    }
}
