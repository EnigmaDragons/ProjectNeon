using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicServiceButton : MonoBehaviour
{
    [SerializeField] private Localize title;
    [SerializeField] private Localize description;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI cost;
    [SerializeField] private Image credIcon;
    [SerializeField] private Button button;
    [SerializeField] private TwoSidedRulesDescriptionPresenter rules;

    public void Init(ClinicServiceButtonData data, PartyAdventureState party)
    {
        title.SetTerm(data.NameTerm);
        description.SetFinalText(data.Description);
        cost.text = data.Cost.ToString();
        button.interactable = data.Enabled && party.Credits >= data.Cost;
        button.onClick.RemoveAllListeners();
        if (party.Credits >= data.Cost)
            button.onClick.AddListener(() =>
            {
                party.UpdateCreditsBy(-data.Cost);
                data.Select();
                Message.Publish(new UpdateClinic());
            });
        if (data.Cost == 0)
        {
            cost.gameObject.SetActive(false);
            credIcon.gameObject.SetActive(false);
        }
        if (rules != null)
        {
            rules.Show(data.RulesContext);
        }
    }
}