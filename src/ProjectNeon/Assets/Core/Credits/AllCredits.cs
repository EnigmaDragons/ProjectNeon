using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class AllCredits : ScriptableObject, ILocalizeTerms
{
    [SerializeField] private RoleCredit[] credits;
    [SerializeField] private GameObject[] additionalCredits;

    public RoleCredit[] Credits => credits;
    public GameObject[] AdditionalCredits => additionalCredits;

    public string[] GetLocalizeTerms()
        => credits.Select(x => $"Credits/{x.role}").ToArray();
}
