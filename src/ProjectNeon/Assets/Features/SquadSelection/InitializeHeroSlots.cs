using System;
using System.Linq;
using UnityEngine;

public class InitializeHeroSlots : MonoBehaviour
{
    [SerializeField] private HeroPool heroPool;
    [SerializeField] private GameObject container;
    [SerializeField] private SquadSlot[] slots;
    [SerializeField] private CurrentAdventure current;

    private void OnEnable()
    {
        heroPool.ClearSelections();
        var availableHeroes = heroPool.AvailableHeroes.Where(x => !current.Adventure.BannedHeroes.Contains(x));
        if (availableHeroes.None())
            throw new InvalidOperationException("No Available Heroes");

        var rng = new DeterministicRng(CurrentGameData.Data.AdventureProgress.RngSeed);
        for (var i = 0; i < 3; i++)
        {
            if (i < current.Adventure.PartySize)
            {
                slots[i].Init(i, current.Adventure.BannedHeroes);
                if (current.Adventure.RequiredHeroes.Length > i)
                    slots[i].SelectRequiredHero(current.Adventure.RequiredHeroes[i]);
                else
                    slots[i].Randomize(rng);
            }
            else 
                slots[i].gameObject.SetActive(false);
        }
    }
}
