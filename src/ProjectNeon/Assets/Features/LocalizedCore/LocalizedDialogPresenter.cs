using System;
using I2.Loc;
using UnityEngine;

public class LocalizedDialogPresenter : MonoBehaviour
{
    [SerializeField] private GameObject darken;
    [SerializeField] private Localize promptLabel;
    [SerializeField] private LocalizedCommandButton primaryButton;
    [SerializeField] private LocalizedCommandButton secondaryButton;
    [SerializeField] private GameObject secondaryButtonPanel;

    public void Show(ShowLocalizedDialog m)
    {
        darken.SetActive(m.UseDarken);
        promptLabel.SetTerm(m.PromptTerm);
        primaryButton.InitTerm(m.PrimaryButtonTerm, () => HideThisAndThen(m.PrimaryAction));
        secondaryButton.InitTerm(m.SecondaryButtonTerm, () => HideThisAndThen(m.SecondaryAction));
        secondaryButtonPanel.SetActive(!string.IsNullOrWhiteSpace(m.SecondaryButtonTerm));
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    private void HideThisAndThen(Action a)
    {
        Hide();
        a();
    }
}
