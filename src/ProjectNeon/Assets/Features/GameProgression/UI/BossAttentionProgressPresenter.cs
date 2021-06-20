using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossAttentionProgressPresenter : OnMessage<AdventureProgressChanged>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private Image barFill;

    private void Awake() => barFill.fillAmount = adventure.ProgressToUnlockChapterBoss;

    private void SmoothTransitionTo(float amount) => barFill.DOFillAmount(amount, 1);

    protected override void Execute(AdventureProgressChanged msg) => SmoothTransitionTo(adventure.ProgressToUnlockChapterBoss);
}
