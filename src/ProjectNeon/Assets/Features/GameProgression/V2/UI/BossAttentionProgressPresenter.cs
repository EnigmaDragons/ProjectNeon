using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossAttentionProgressPresenter : OnMessage<AdventureProgressChanged, NodeFinished>
{
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private Image barFill;
    [SerializeField] private GameObject heatUpMarkerPrototype;
    [SerializeField] private GameObject heatUpMarketParent;
    [SerializeField] private float heatUpPlacementFactor = 1f;

    private readonly float _visualFactor = 1f;
    private readonly float _offsetAmount = 0f;

    private void Awake()
    {
        Log.Info($"Stage Progress {adventure.ProgressToUnlockChapterBoss}");
        barFill.fillAmount = FillAmount;
        RenderHeatUpMarkers();
    }

    private void RenderHeatUpMarkers()
    {
        heatUpMarketParent.DestroyAllChildren();
        var heatUps = adventure.RemainingHeatUpEvents;
        heatUps.ForEach(h =>
        {
            var o = Instantiate(heatUpMarkerPrototype, heatUpMarketParent.transform);
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(h.Value.ProgressThreshold * heatUpPlacementFactor * _visualFactor, 0);
        });
    }

    private void SmoothTransitionTo(float amount) => barFill.DOFillAmount(amount * _visualFactor + _offsetAmount, 1);

    protected override void Execute(AdventureProgressChanged msg) => RenderUpdate();

    private void RenderUpdate()
    {
        Log.Info($"Updated Adventure Stage Progress Bar - {adventure.ProgressToUnlockChapterBoss}");
        SmoothTransitionTo(FillAmount);
        RenderHeatUpMarkers();
    }

    protected override void Execute(NodeFinished msg) => this.ExecuteAfterTinyDelay(RenderUpdate);

    private float FillAmount => adventure.ProgressToUnlockChapterBoss * _visualFactor + _offsetAmount;
}
