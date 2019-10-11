using UnityEngine;

public sealed class SetupPartyForGame : MonoBehaviour
{
    [SerializeField] private HeroPool pool;
    [SerializeField] private Party party;

    public void SetupParty()
    {
        var heroes = pool.SelectedHeroes;
        party.Initialized(heroes[0], heroes[1], heroes[2]);
    }
}
