
using TMPro;
using UnityEngine;

[IgnoreForLocalization]
public class CreditPresenter : CreditPresenterBase
{
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI roleLabel;

    public override CreditPresenterBase Initialized(RoleCredit credit)
    {
        nameLabel.text = credit.personName;
        roleLabel.text = credit.role.ToUpperInvariant();
        return this;
    }
}
