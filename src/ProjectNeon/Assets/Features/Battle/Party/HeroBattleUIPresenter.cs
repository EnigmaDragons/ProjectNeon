using UnityEngine;
using UnityEngine.UI;

public class HeroBattleUIPresenter : MonoBehaviour
{
    [SerializeField] private Image bust;
    [SerializeField] private UIHPBarController hp;
    [SerializeField] private BattleState state;
    [SerializeField] private ResourceCounterPresenter resource1;
    [SerializeField] private ResourceCounterPresenter resource2;
    
    public void Set(Hero hero)
    {
        bust.sprite = hero.Bust;
        hp.Init((int)state.GetMemberByHero(hero).State.MaxHP());
        InitResources(hero.Stats.ResourceTypes);
    }

    private void InitResources(IResourceType[] resources)
    {
        if (resources.Length > 0 && resources[0].MaxAmount > 0)
            resource1.Init(resources[0]);
        else
            resource1.Hide();

        if (resources.Length > 1 && resources[1].MaxAmount > 0)
            resource2.Init(resources[1]);
        else
            resource2.Hide();
    }
}
