using UnityEngine;

public class CorpLightDetailUiController : OnMessage<ShowCorpLightDetail, HideCorpDetails>
{
    [SerializeField] private CorpUiBase[] uiViews;
    
    protected override void Execute(ShowCorpLightDetail msg)
    {
        uiViews.ForEach(u =>
        {
            u.Init(msg.Corp);
            u.gameObject.SetActive(true);
        });
    }

    protected override void Execute(HideCorpDetails msg)
    {
        uiViews.ForEach(u => u.gameObject.SetActive(false));
    }
}