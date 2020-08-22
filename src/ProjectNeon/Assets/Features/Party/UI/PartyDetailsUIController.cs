
using UnityEngine;

public class PartyDetailsUIController : OnMessage<TogglePartyDetails>
{
    [SerializeField] private GameObject target;

    protected override void Execute(TogglePartyDetails msg) => target.SetActive(!target.activeSelf);
}
