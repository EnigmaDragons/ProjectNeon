using UnityEngine;

public class PatchNotesUIController : OnMessage<TogglePatchNotes>
{
    [SerializeField] private GameObject target;

    private void Awake() => target.SetActive(false);

    protected override void Execute(TogglePatchNotes msg) => target.SetActive(!target.activeSelf);
}
