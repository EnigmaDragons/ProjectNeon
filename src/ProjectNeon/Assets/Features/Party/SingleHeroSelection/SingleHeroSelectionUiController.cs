using UnityEngine;

public class SingleHeroSelectionUiController : OnMessage<GetUserSelectedHero>
{
    [SerializeField] private GameObject target;
    [SerializeField] private SingleHeroSelectionUiPresenter presenter;
    
    protected override void Execute(GetUserSelectedHero msg)
    {
        target.SetActive(true);
        presenter.Initialized(msg, () =>
        {
            Message.Publish(new NodeFinished());
        });
    }
}
