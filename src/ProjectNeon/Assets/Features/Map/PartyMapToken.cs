using UnityEngine;
using UnityEngine.UI;

public class PartyMapToken : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private PartyAdventureState party;

    void Awake() => heroBust.sprite = party.BaseHeroes[0].Bust;
}
