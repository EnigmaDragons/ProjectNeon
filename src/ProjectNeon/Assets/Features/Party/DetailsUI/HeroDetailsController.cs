using UnityEngine;

public class HeroDetailsController : OnMessage<ShowHeroDetailsView, HideHeroDetailsView>
{
    [SerializeField] private GameObject target;
    [SerializeField] private HeroDetailsView heroDetails;
    
    protected override void Execute(ShowHeroDetailsView msg)
    {
        target.SetActive(true);
        heroDetails.Init(msg.Hero);
    }

    protected override void Execute(HideHeroDetailsView msg)
    {
        target.SetActive(false);
    }
}
