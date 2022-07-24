using System.Linq;
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

    public CompletionProgressPresenter Initialized(Adventure adv, ProgressionItem[] progressItems)
        => Initialized(adv.MapTitle, adv.AdventureImage, progressItems, adv.IsLocked ? adv.LockConditionExplanation : Maybe<string>.Missing());
    
    public CompletionProgressPresenter Initialized(string title, Sprite coverArt, ProgressionItem[] progressItems, Maybe<string> lockReason)
    {
        adventureCover.sprite = coverArt;
        adventureTitle.text = title;
        RenderHeroes(progressItems);
        completionCheckmark.SetActive(progressItems.All(p => p.Completed));
        lockedObj.SetActive(lockReason.IsPresent);
        if (lockReason.IsPresent)
            lockReasonLabel.text = lockReason.Value;
        gameObject.SetActive(true);
        return this; 
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
