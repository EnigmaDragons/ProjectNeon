using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class AllCredits : ScriptableObject, ILocalizeTerms
{
    [SerializeField] private RoleCredit[] credits;

    public RoleCredit[] Credits => credits;

    public string[] GetLocalizeTerms()
        => credits.Select(x => $"Credits/{x.role}").ToArray();
}
