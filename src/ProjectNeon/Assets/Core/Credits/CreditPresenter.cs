
using TMPro;
using UnityEngine;

public class CreditPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI roleLabel;

    public CreditPresenter Initialized(RoleCredit credit)
    {
        nameLabel.text = credit.personName;
        roleLabel.text = credit.role.ToUpperInvariant();
        return this;
    }
}
