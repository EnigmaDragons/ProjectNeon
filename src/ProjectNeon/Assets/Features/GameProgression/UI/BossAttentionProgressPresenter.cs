using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossAttentionProgressPresenter : OnMessage<AdventureProgressChanged>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private Image barFill;

    private readonly float _visualFactor = 0.95f;
    private readonly float _offsetAmount = 0.05f;

    private void Awake()
    {
        Log.Info($"Stage Progress {adventure.ProgressToUnlockChapterBoss}");
        barFill.fillAmount = FillAmount;
    }

    private void SmoothTransitionTo(float amount) => barFill.DOFillAmount(amount * _visualFactor + _offsetAmount, 1);

    protected override void Execute(AdventureProgressChanged msg)
    {
        Log.Info($"Stage Progress {adventure.ProgressToUnlockChapterBoss}");
        SmoothTransitionTo(FillAmount);
    }

    private float FillAmount => adventure.ProgressToUnlockChapterBoss * _visualFactor + _offsetAmount;
}
