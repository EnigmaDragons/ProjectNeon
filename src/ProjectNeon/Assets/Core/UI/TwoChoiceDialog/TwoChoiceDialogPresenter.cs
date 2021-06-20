using System;
using TMPro;
using UnityEngine;

public class TwoChoiceDialogPresenter : MonoBehaviour
{
    [SerializeField] private GameObject darken;
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private TextCommandButton primaryButton;
    [SerializeField] private TextCommandButton secondaryButton;

    public void Show(ShowTwoChoiceDialog m)
    {
        darken.SetActive(m.UseDarken);
        promptLabel.text = m.Prompt;
        primaryButton.Init(m.PrimaryButtonText, () => HideThisAndThen(m.PrimaryAction));
        secondaryButton.Init(m.SecondaryButtonText, () => HideThisAndThen(m.SecondaryAction));
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);

    private void HideThisAndThen(Action a)
    {
        Hide();
        a();
    }
}