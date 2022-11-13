using UnityEngine;

public class OnlyEnabledIfLanguageNotSelected : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Start() => target.SetActive(!CurrentAcademyData.Data.HasSelectedLanguage);
}
