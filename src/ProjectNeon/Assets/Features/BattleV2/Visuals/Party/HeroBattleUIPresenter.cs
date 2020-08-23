using UnityEngine;
using UnityEngine.UI;

public class HeroBattleUIPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    [SerializeField] private UIHPBarController hp;
    [SerializeField] private BattleState state;
    [SerializeField] private ResourceCounterPresenter resource1;
    [SerializeField] private ResourceCounterPresenter resource2;
    [SerializeField] private StatusBar statusBar;

    private BaseHero _hero;

    public bool Contains(string heroName) => _hero.Name.Equals(heroName);
    
    public void Set(BaseHero hero)
    {
        _hero = hero;
        var member = state.GetMemberByHero(hero);
        bust.sprite = hero.Bust;
        hp.Init(member);
        InitResources(member, hero.Stats.ResourceTypes);
        statusBar.Initialized(member);
    }

    private void InitResources(Member member, IResourceType[] resources)
    {
        if (resources.Length > 0 && resources[0].MaxAmount > 0)
            resource1.Init(member, resources[0]);
        else
            resource1.Hide();

        if (resources.Length > 1 && resources[1].MaxAmount > 0)
            resource2.Init(member, resources[1]);
        else
            resource2.Hide();
    }
}
