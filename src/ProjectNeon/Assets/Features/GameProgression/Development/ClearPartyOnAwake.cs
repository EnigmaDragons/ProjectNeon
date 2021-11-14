using UnityEngine;

public class ClearPartyOnAwake : MonoBehaviour
{
    [SerializeField] private BaseHero noHero;
    [SerializeField] private PartyAdventureState party;

    private void Awake() => party.Initialized(noHero, noHero, noHero);
}
