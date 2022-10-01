using UnityEngine;

public class DisabledIfTutorial : MonoBehaviour
{
    [SerializeField] private GameObject target;
    
    private void Start()
    {
        var t = target != null ? target : gameObject;
        t.SetActive(CurrentAcademyData.Data.IsLicensedBenefactor);
    }
}
