using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AdventureProgressV5Presenter : OnMessage<AdventureProgressChanged>
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
        Log.Info($"Stage Progress {adventure.AdventureProgress.ProgressToBoss}");
        barFill.fillAmount = FillAmount;
        RenderMarkers();
    }

    private void RenderMarkers()
    {
        markerParent.DestroyAllChildren();
        return;
        //
        // var heatUps = adventure.RemainingHeatUpEvents;
        // heatUps.ForEach(h =>
        // {
        //     var o = Instantiate(heatUpMarkerPrototype, heatUpMarketParent.transform);
        //     o.GetComponent<RectTransform>().anchoredPosition = new Vector2(h.Value.ProgressThreshold * heatUpPlacementFactor * _visualFactor, 0);
        // });
    }

    private void SmoothTransitionTo(float amount) => barFill.DOFillAmount(amount * _visualFactor + _offsetAmount, 1);

    protected override void Execute(AdventureProgressChanged msg)
    {
        Log.Info($"Stage Progress {adventure.AdventureProgress.ProgressToBoss}");
        SmoothTransitionTo(FillAmount);
        RenderMarkers();
    }

    private float FillAmount => adventure.AdventureProgress.ProgressToBoss * _visualFactor + _offsetAmount;
}