using UnityEngine;
using UnityEngine.UI;

public class PartyHeroBusts : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private Image[] bustImages;
    
    private void OnEnable()
    {
        var heroes = party.BaseHeroes;
        bustImages.ForEach(i => i.enabled = false);
        for (var i = 0; i < bustImages.Length && i < heroes.Length; i++)
        {
            bustImages[i].enabled = true;
            bustImages[i].sprite = heroes[i].Bust;
        }
    }
}