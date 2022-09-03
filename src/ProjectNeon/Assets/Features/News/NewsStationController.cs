using UnityEngine;

public class NewsStationController : OnMessage<ShowNewscast, HideNewscast>
{
    [SerializeField] private GameObject newscastCanvas;

    private void Awake() => newscastCanvas.SetActive(false);
    
    protected override void Execute(ShowNewscast msg) => newscastCanvas.SetActive(true);

    protected override void Execute(HideNewscast msg) => newscastCanvas.SetActive(false);
}
