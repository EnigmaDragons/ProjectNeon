
using UnityEngine;

public class ClinicUIController : OnMessage<ToggleClinic>
{
    [SerializeField] private GameObject clinicUi;
    
    protected override void Execute(ToggleClinic msg) => clinicUi.SetActive(!clinicUi.activeSelf);
}
