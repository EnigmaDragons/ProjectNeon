using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;

public class SimpleCardsUI : MonoBehaviour
{
    [SerializeField] private GameObject largeZoneParent;
    [SerializeField] private GameObject mediumZoneParent;
    [SerializeField] private GameObject smallZoneParent;
    [SerializeField] private CardInLibraryButton cardPrototype;
    [SerializeField] private GameObject noCardsInZone;
    [SerializeField] private Localize headerLabel;
    
    public void Init(string headerTerm, Card[] cards)
    {
        headerLabel.SetTerm(headerTerm);
        ClearZones();
        noCardsInZone.SetActive(cards.Length == 0);
        
        var sortedCards = cards
            .OrderBy(x => x.Owner.Id)
            .ThenBy(x => x.Cost.CostSortOrder())
            .ThenBy(x => x.GetArchetypeKey())
            .ThenBy(x => x.Name);

        var selectedParent = largeZoneParent;
        if (cards.Length <= 24)
            selectedParent = mediumZoneParent;
        if (cards.Length <= 12)
            selectedParent = smallZoneParent;
        foreach (var c in sortedCards)
            Instantiate(cardPrototype, selectedParent.transform).InitInfoOnly(c, () => { });
        selectedParent.SetActive(true);
    }

    private void ClearZones()
    {
        largeZoneParent.DestroyAllChildren();
        largeZoneParent.SetActive(false);
        mediumZoneParent.DestroyAllChildren();
        mediumZoneParent.SetActive(false);
        smallZoneParent.DestroyAllChildren();
        smallZoneParent.SetActive(false);
    }
}
