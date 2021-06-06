using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class LevelUpOptionPresenter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image button;
    [SerializeField] private Sprite hoverImg;

    private Sprite _defaultImg;
    private HeroLevelUpOption _option;

    private void Awake() => _defaultImg = button.sprite;
    private void OnEnable() => button.sprite = _defaultImg;

    public LevelUpOptionPresenter Initialized(HeroLevelUpOption o)
    {
        _option = o;
        text.text = o.Description.Replace("\\n", "\n");
        return this;
    }

    private void SelectLevelUpOption()
    {
        if (_option != null)
            Message.Publish(new LevelUpOptionSelected(_option));
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            SelectLevelUpOption();
        if (eventData.button == PointerEventData.InputButton.Right)
            _option.ShowDetail();
    }

    public void OnPointerEnter(PointerEventData eventData) => button.sprite = hoverImg;
    public void OnPointerExit(PointerEventData eventData) => button.sprite = _defaultImg;
}
