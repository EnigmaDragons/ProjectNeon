
using UnityEngine;

public class ClinicUIController : OnMessage<ToggleClinic>
{
    [SerializeField] private GameObject clinicUi;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private AllCorps corps;

    protected override void Execute(ToggleClinic msg)
    {
        clinic.Corp = corps.GetCorpByNameOrNone(msg.CorpName);
        clinicUi.SetActive(!clinicUi.activeSelf);
    } 
}
