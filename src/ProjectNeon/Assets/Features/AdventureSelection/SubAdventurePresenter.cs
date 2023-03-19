using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SubAdventurePresenter : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Localize text;
    [SerializeField] private Image button;
    [SerializeField] private Sprite hoverImg;
    [SerializeField] private GameObject lockVisual;

    private bool _isLocked;
    private Sprite _defaultImg;
    private Action _onSelect;
    
    private void Awake() => _defaultImg = button.sprite;
    private void OnEnable() => button.sprite = _defaultImg;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isLocked)
            button.sprite = hoverImg;
    } 
    public void OnPointerExit(PointerEventData eventData) => button.sprite = _defaultImg;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isLocked)
            _onSelect();
    }
    
    public void Init(Adventure adventure, Action onSelect)
    {
        _isLocked = adventure.IsLocked;
        lockVisual.SetActive(adventure.IsLocked);
        text.SetTerm(adventure.StoryTerm);
        _onSelect = onSelect;
    }
}