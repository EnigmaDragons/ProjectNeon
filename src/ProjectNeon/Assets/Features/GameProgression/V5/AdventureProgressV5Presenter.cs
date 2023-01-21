using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AdventureProgressV5Presenter : OnMessage<AdventureProgressChanged, NodeFinished>
{
    [SerializeField] private CurrentAdventureProgress adventure;
    [SerializeField] private Image barFill;
    [SerializeField] private GameObject minibossMarkerPrototype;
    [SerializeField] private GameObject eliteMarkerPrototype;
    [SerializeField] private GameObject markerParent;
    [SerializeField] private float markerPlacementFactor = 1f;

    private readonly float _visualFactor = 1f;
    private readonly float _offsetAmount = 0f;

    private void Awake()
    {
        RenderUpdateNoAnim();
    }

    private void RenderUpdateNoAnim()
    {
        Log.Info($"Stage Progress {adventure.AdventureProgress.ProgressToBoss}");
        barFill.fillAmount = FillAmount;
        RenderMarkers();
    }

    private void RenderMarkers()
    {
        markerParent.DestroyAllChildren();

        var progress = adventure.AdventureProgress.ProgressToBoss;
        var risingActions = adventure.AdventureProgress.RisingActionPoints;
        risingActions.Where(h => h > progress).ForEach(h =>
        {
            var o = Instantiate(eliteMarkerPrototype, markerParent.transform);
            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(h * markerPlacementFactor * _visualFactor, 0);
        });
    }

    private void SmoothTransitionTo(float amount) => barFill.DOFillAmount(amount * _visualFactor + _offsetAmount, 1);

    protected override void Execute(AdventureProgressChanged msg) => RenderUpdateWithAnim();

    private void RenderUpdateWithAnim()
    {
        Log.Info($"Stage Progress {adventure.AdventureProgress.ProgressToBoss}");
        SmoothTransitionTo(FillAmount);
        RenderMarkers();
    }

    protected override void Execute(NodeFinished msg) => this.ExecuteAfterTinyDelay(RenderUpdateWithAnim);

    private float FillAmount => adventure.AdventureProgress.ProgressToBoss * _visualFactor + _offsetAmount;
}