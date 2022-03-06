using UnityEngine;

public class RequiresBenefactorLicense : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    
    private void Awake()
    {
#if !UNITY_EDITOR
        if (!CurrentAcademyData.Data.IsLicensedBenefactor)
            navigator.NavigateToAcademyScene();
#endif
    }
}
