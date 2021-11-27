using UnityEngine;

public class InitPartyOnAwake : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private BaseHero[] heroes;

    private void Awake() => party.Initialized(heroes);
}
