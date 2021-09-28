using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class LevelUpOptionPresenter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image button;
    [SerializeField] private Sprite hoverImg;
    [SerializeField] private GameObject hasDetailPrompt;

    private Sprite _defaultImg;
    private HeroLevelUpOption _option;
    private HeroLevelUpOption[] _allOptions;

    private void Awake() => _defaultImg = button.sprite;
    private void OnEnable() => button.sprite = _defaultImg;

    public LevelUpOptionPresenter Initialized(HeroLevelUpOption o, HeroLevelUpOption[] allOptions)
    {
        _option = o;
        _allOptions = allOptions;
        text.text = o.Description.Replace("\\n", "\n");
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
}
