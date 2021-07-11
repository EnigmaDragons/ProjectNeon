using TMPro;
using UnityEngine;

public class ClinicUI : MonoBehaviour
{
    [SerializeField] private GameObject patientParent;
    [SerializeField] private ClinicPatientUI patientPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ClinicState clinic;
    [SerializeField] private CorpClinicCostCalculators clinics;

    private void OnDisable() => Message.Publish(new AutoSaveRequested());
    private void OnEnable()
    {
        patientParent.DestroyAllChildren();
        party.Heroes.ForEach(h => Instantiate(patientPrototype, patientParent.transform).Initialized(h, clinics.Get(clinic.Corp)));
    }
}
