
using UnityEngine;

public class PartyDetailsUIController : OnMessage<TogglePartyDetails>
{
    [SerializeField] private PartyDetailsUI target;

    protected override void Execute(TogglePartyDetails msg)
    {
        target.gameObject.SetActive(!target.gameObject.activeSelf);
        if (target.gameObject.activeSelf && !msg.AllowDone)
            target.UseNextButtonOneTime();
    }
}
