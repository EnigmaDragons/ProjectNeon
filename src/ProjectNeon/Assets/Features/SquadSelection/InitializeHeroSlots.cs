using System;
using UnityEngine;

public class InitializeHeroSlots : MonoBehaviour
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private GameObject container;
    [SerializeField] private SquadSlot slotPrefab;
    [SerializeField] private AdventureProgress adventureProgress;

    private void OnEnable()
    {
        heroPool.ClearSelections();
        if (heroPool.AvailableHeroes.None())
            throw new InvalidOperationException("No Available Heroes");
        for (var i = 0; i < adventureProgress.Adventure.PartySize; i++)
        {
            var s = Instantiate(slotPrefab, container.transform);
            s.Init(i);
            if (adventureProgress.Adventure.RequiredHeroes.Length > i)
                s.SelectRequiredHero(adventureProgress.Adventure.RequiredHeroes[i]);
        }
    }
}
