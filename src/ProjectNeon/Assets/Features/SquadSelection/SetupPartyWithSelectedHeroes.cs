using UnityEngine;

public class SetupPartyWithSelectedHeroes : MonoBehaviour
{
    [SerializeField] private HeroPool pool;
    [SerializeField] private PartyAdventureState party;

    public void SetupParty()
    {
        var heroes = pool.SelectedHeroes;
        party.Initialized(heroes[0], heroes[1], heroes[2]);
    }
}
