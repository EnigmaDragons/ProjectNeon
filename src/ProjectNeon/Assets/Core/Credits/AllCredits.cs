using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class AllCredits : ScriptableObject
{
    [SerializeField] private RoleCredit[] credits;

    public RoleCredit[] Credits => credits;
}
