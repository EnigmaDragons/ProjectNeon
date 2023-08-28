using I2.Loc;
using TMPro;
using UnityEngine;

public class I2SlideTextPresenter : SlideTextPresenterBase
{
    [SerializeField] private Localize slideLocalize;

    protected override void InitText(string term) => slideLocalize.SetTerm(term);
}