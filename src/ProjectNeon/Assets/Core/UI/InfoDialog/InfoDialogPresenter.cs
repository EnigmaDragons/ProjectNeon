using TMPro;
using UnityEngine;

public class InfoDialogPresenter : MonoBehaviour
{
    [SerializeField] private GameObject darken;
    [SerializeField] private TextMeshProUGUI infoLabel;
    [SerializeField] private TextCommandButton doneButton;

    public void Show(ShowInfoDialog m)
    {
        darken.SetActive(m.UseDarken);
        infoLabel.text = m.Info;
        doneButton.Init(m.DoneButtonText, Hide);
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}
