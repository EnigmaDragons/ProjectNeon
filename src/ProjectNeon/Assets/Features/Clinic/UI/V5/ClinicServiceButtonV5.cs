using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClinicServiceButtonV5 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cost;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private Button button;
    [SerializeField] private TwoSidedRulesDescriptionPresenter rules;
    [SerializeField] private CorpUiBase[] corpUi;
    [SerializeField] private AllCorps corps;
    [SerializeField] private CanvasGroup disabledCanvasGroup;

    private ClinicServiceButtonData _data;
    
    public void Init(ClinicServiceButtonData data, PartyAdventureState party)
    {
        _data = data;
        title.text = data.Name;
        description.text = data.Description;
        cost.text = data.Cost.ToString();
        var interactable = data.Enabled && party.ClinicVouchers >= data.Cost;
        button.interactable = interactable;
        if (disabledCanvasGroup != null)
            disabledCanvasGroup.alpha = interactable ? 1 : 0.3f;
        button.onClick.RemoveAllListeners();
        if (party.ClinicVouchers >= data.Cost)
            button.onClick.AddListener(() =>
            {
                party.UpdateClinicVouchersBy(-data.Cost);
                data.Action();
                Message.Publish(new UpdateClinic());
            });
        if (data.Cost == 0)
        {
            cost.gameObject.SetActive(false);
            currencyIcon.gameObject.SetActive(false);
        }
        corps.GetCorpByName(data.CorpName)
            .IfPresent(c => corpUi.ForEach(cui => cui.Init(c)));
    }

    public void ShowRules()
    {
        if (rules != null)
            rules.Show(_data.RulesContext);
    }

    public void HideRules()
    {
        if (rules != null)
            rules.Hide();
    }
}
