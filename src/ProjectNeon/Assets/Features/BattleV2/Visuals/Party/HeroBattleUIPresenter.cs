using UnityEngine;
using UnityEngine.UI;

public class HeroBattleUIPresenter : OnMessage<SetHeroesUiVisibility>
{
    [SerializeField] private Image bust;
    [SerializeField] private UIHPBarController hp;
    [SerializeField] private BattleState state;
    [SerializeField] private ResourceCounterPresenter resource1;
    [SerializeField] private ResourceCounterPresenter resource2;
    [SerializeField] private StatusBar statusBar;
    [SerializeField] private UiStatPresenter primaryStat;
    [SerializeField] private GameObject statPresenter;

    private bool _hasResource1;
    private bool _hasResource2;
    private BaseHero _hero;

    public bool Contains(string heroNameTerm) => _hero.NameTerm().Equals(heroNameTerm);
    
    public void Set(BaseHero hero, bool primaryStatVisible, bool resourcesVisible, bool shieldsVisible)
    {
        _hero = hero;
        var member = state.GetMemberByHero(hero);
        bust.sprite = hero.Bust;
        hp.Init(member);
        InitResources(member, hero.Stats.ResourceTypes, resourcesVisible);
        statusBar.Initialized(member);
        primaryStat.Init(member, member.PrimaryStat());
        statPresenter.SetActive(primaryStatVisible);
        hp.SetShieldsVisibility(shieldsVisible);
    }

    private void InitResources(Member member, IResourceType[] resources, bool resourcesVisible)
    {
        _hasResource1 = resources.Length > 0 && resources[0].MaxAmount > 0;
        _hasResource2 = resources.Length > 1 && resources[1].MaxAmount > 0;
        if (_hasResource1)
            resource1.Init(member, resources[0]);
        else
            resource1.Hide();

        if (_hasResource2)
            resource2.Init(member, resources[1]);
        else
            resource2.Hide();
        
        if (_hasResource1)
            resource1.gameObject.SetActive(resourcesVisible);
        if (_hasResource2)
            resource2.gameObject.SetActive(resourcesVisible);
    }

    protected override void Execute(SetHeroesUiVisibility msg)
    {
        if (msg.Component == BattleUiElement.PrimaryStat)
            statPresenter.SetActive(msg.ShouldShow);
        else if (msg.Component == BattleUiElement.PlayerResources)
        {
            if (_hasResource1)
                resource1.gameObject.SetActive(msg.ShouldShow);
            if (_hasResource2)
                resource2.gameObject.SetActive(msg.ShouldShow);
        }
        else if (msg.Component == BattleUiElement.PlayerShields)
            hp.SetShieldsVisibility(msg.ShouldShow);
    }
}
