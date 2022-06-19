using UnityEngine;

public class EnabledIfTutorial : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(!CurrentAcademyData.Data.IsLicensedBenefactor);
    }
}