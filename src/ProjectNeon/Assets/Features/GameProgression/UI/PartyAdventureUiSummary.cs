using System.Collections.Generic;
using UnityEngine;

public class PartyAdventureUiSummary : MonoBehaviour
{
    [SerializeField] private CurrentAdventure current;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private AdventureHeroUiSummary heroPresenter;
    [SerializeField] private GameObject itemParent;
    
    [ReadOnly, SerializeField] private List<AdventureHeroUiSummary> active = new List<AdventureHeroUiSummary>();
    
    private void Awake()
    {
        active.ForEach(Destroy);
        active.Clear();
        itemParent.DestroyAllChildren();
        
        for (var i = 0; i < party.BaseHeroes.Length; i++)
        {
            var h = Instantiate(heroPresenter, itemParent.transform);
            h.Init(party.Heroes[i], !current.Adventure.IsV2);
            active.Add(h);
        }
    }
}
