using I2.Loc;
using TMPro;
using UnityEngine;

public class I2CreditPresenter : CreditPresenterBase
{
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI nameLabel;
    [SerializeField] private Localize roleLabel;

    public override CreditPresenterBase Initialized(RoleCredit credit)
    {
        nameLabel.text = credit.personName;
        roleLabel.SetTerm($"Credits/{credit.role}");
        return this;
    }
}