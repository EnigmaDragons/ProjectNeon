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
        for (var i = 0; i < slots.Length; i++)
        {
            var s = slots[i];
            s.Init(i);
            s.SelectNextHero();
            s.SelectPreviousHero();
        }
    }
}
