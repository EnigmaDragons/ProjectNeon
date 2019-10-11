using System;
using UnityEngine;

public class InitializeHeroSlots : MonoBehaviour
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private SquadSlot[] slots;

    private void OnEnable()
    {
        heroPool.ClearSelections();
        if (heroPool.AvailableHeroes.None())
            throw new InvalidOperationException("No Available Heroes");
        slots.ForEach(s =>
        {
            s.SelectNextHero();
            s.SelectPreviousHero();
        });
    }
}
