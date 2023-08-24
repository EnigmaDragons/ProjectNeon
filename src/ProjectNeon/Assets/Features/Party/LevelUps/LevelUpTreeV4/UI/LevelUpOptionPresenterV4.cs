using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class LevelUpOptionPresenterV4 : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Localize text;
    [SerializeField] private Image button;
    [SerializeField] private Sprite hoverImg;
    [SerializeField] private GameObject hasDetailPrompt;
    [SerializeField] private ConfirmActionComponent confirm;
    [SerializeField] private InspectActionComponent inspect;

    private Sprite _defaultImg;
    private LevelUpOption _option;
    private LevelUpOption[] _allOptions;

    private void Awake()
    {
        _defaultImg = button.sprite;
        confirm.Bind(() =>
        {
            SelectLevelUpOption();
            Message.Publish(new LevelUpClicked(transform));
        });
        inspect.Bind(() =>
        {
            Message.Publish(new LevelUpClicked(transform));
            _option.ShowDetail();
        });
    }
    private void OnEnable() => button.sprite = _defaultImg;

    public LevelUpOptionPresenterV4 Initialized(LevelUpOption o, LevelUpOption[] allOptions)
    {
        _option = o;
        _allOptions = allOptions;
        text.SetFinalText(o.Description.Replace("\\n", "\n"));
        hasDetailPrompt.SetActive(o.HasDetail);
        return this;
    }

    private void SelectLevelUpOption()
    {
        if (_option != null)
            Message.Publish(new LevelUpOptionSelected(_option, _allOptions));
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            SelectLevelUpOption();
        Message.Publish(new LevelUpClicked(transform));
        if (eventData.button == PointerEventData.InputButton.Right)
            _option.ShowDetail();
    }

    public void OnPointerEnter(PointerEventData eventData) => button.sprite = hoverImg;
    public void OnPointerExit(PointerEventData eventData) => button.sprite = _defaultImg;
    public void OnSelect(BaseEventData eventData) => button.sprite = hoverImg;
    public void OnDeselect(BaseEventData eventData) => button.sprite = _defaultImg;
}