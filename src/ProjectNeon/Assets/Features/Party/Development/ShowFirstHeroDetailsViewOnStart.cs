using UnityEngine;

public class ShowFirstHeroDetailsViewOnStart : MonoBehaviour
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private StaticEquipment[] equips;
    
    private void Start()
    {
        var h = party.Heroes[0];
        equips.ForEach(e => h.ApplyPermanent(e));
        Message.Publish(new ShowHeroDetailsView(h, Maybe<Member>.Missing()));
    }
}
