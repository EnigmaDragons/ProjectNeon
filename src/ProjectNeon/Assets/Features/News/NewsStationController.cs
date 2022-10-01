using System;
using UnityEngine;

public class NewsStationController : OnMessage<ShowNewscast, HideNewscast>
{
    [SerializeField] private GameObject newscastCanvas;
    [SerializeField] private CurrentCutscene currentCutscene;
    [SerializeField] private NewsCutscenePresenter newsPresenter;

    private void Awake() => Hide();
    
    protected override void Execute(ShowNewscast msg)
    {
        currentCutscene.Init(msg.Cutscene, Maybe<Action>.Present(Hide));
        newscastCanvas.SetActive(true);
        newsPresenter.StartNewscast();
    }

    protected override void Execute(HideNewscast msg) => Hide();
    
    private void Hide() => newscastCanvas.SetActive(false);
}
