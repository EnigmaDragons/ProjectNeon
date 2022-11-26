using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClinicServiceButtonV5 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Localize title;
    [SerializeField] private Localize descriptionLocalize;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI cost;
    [SerializeField] private Image currencyIcon;
    [SerializeField] private Button button;
    [SerializeField] private TwoSidedRulesDescriptionPresenter rules;
    [SerializeField] private CorpUiBase[] corpUi;
    [SerializeField] private AllCorps corps;
    [SerializeField] private CanvasGroup disabledCanvasGroup;
    [SerializeField] private RarityPresenter rarity;
    [SerializeField] private GameObject highlight;

    private ClinicServiceButtonData _data;
    
    public void Init(ClinicServiceButtonData data, PartyAdventureState party)
    {
        _data = data;
        title.SetTerm(data.NameTerm);
        descriptionLocalize.SetFinalText(data.Description);
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
        if (data.Rarity == Rarity.Starter)
            rarity.gameObject.SetActive(false);
        else 
            rarity.Set(data.Rarity);
        highlight.SetActive(false);
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowRules();
        highlight.SetActive(true);
        Message.Publish(new ItemHovered(transform));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideRules();
        highlight.SetActive(false);
    }
}
