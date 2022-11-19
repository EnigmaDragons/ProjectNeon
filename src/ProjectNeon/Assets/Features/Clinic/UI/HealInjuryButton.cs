using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HealInjuryButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Localize injuryLabel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Button _button;

    private string _injuryTooltip;
    
    public void Init(HeroInjury injury, int cost, Action action)
    {
        injuryLabel.SetTerm($"Injuries/{injury.InjuryName}");
        _injuryTooltip = injury.Description;
        costLabel.text = cost.ToString();
        _button.onClick.AddListener(() => action());
    }

    public void OnPointerEnter(PointerEventData eventData) => Message.Publish(new ShowTooltip(transform, _injuryTooltip));
    public void OnPointerExit(PointerEventData eventData) => Message.Publish(new HideTooltip());
}