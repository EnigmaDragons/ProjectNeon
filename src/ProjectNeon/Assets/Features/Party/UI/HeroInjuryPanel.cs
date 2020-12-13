using UnityEngine;

public class HeroInjuryPanel : MonoBehaviour
{
    [SerializeField] private GameObject injuryParent;
    [SerializeField] private InjuryDetailView injuryDetailViewPrototype;
    
    private Hero _hero;

    private void Awake()
    {
        injuryParent.DestroyAllChildren();
    }
    
    public void Init(Hero h)
    {
        _hero = h;
        injuryParent.DestroyAllChildren();
        _hero.Health.InjuryCounts.ForEach(x => Instantiate(injuryDetailViewPrototype, injuryParent.transform)
            .Init(x.Value, x.Key.InjuryName, x.Key.Description));
    }
}
