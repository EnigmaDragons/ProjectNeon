using System;
using System.Linq;
using UnityEngine;

public class InitializeHeroSlots : MonoBehaviour
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private GameObject container;
    [SerializeField] private SquadSlot slotPrefab;
    [SerializeField] private CurrentAdventure current;

    private void Awake() => gameObject.DestroyAllChildren();
    
    private void OnEnable()
    {
        heroPool.ClearSelections();
        var availableHeroes = heroPool.AvailableHeroes.Where(x => !current.Adventure.BannedHeroes.Contains(x));
        if (availableHeroes.None())
            throw new InvalidOperationException("No Available Heroes");
        
        for (var i = 0; i < current.Adventure.PartySize; i++)
        {
            var s = Instantiate(slotPrefab, container.transform);
            s.Init(i, current.Adventure.BannedHeroes);
            if (current.Adventure.RequiredHeroes.Length > i)
                s.SelectRequiredHero(current.Adventure.RequiredHeroes[i]);
            if (current.Adventure.PartySize >= heroPool.TotalHeroesCount)
                s.SetNoChoicesAvailable();
        }
    }
}
