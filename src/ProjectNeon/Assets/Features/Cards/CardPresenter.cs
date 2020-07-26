using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private ClassTints classTints;
    [SerializeField] private BattleState battleState;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI type;
    [SerializeField] private GameObject costPanel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Image costResourceTypeIcon;
    [SerializeField] private Image art;
    [SerializeField] private Image tint;
    [SerializeField] private GameObject canPlayHighlight;
    [SerializeField] private GameObject highlight;
    [SerializeField] private float highlightedScale = 1.7f;

    private Card _card;
    private Action _onClick;

    public bool Contains(Card c) => HasCard && _card == c;
    public bool HasCard => _card != null;
    public bool IsPlayable => canPlayHighlight.activeSelf;

    public void ClearIfIs(Card c)
    {
        if (Contains(c))
            Clear();
    }
    
    public void Clear()
    {
        gameObject.SetActive(false);
        _card = null;
    }

    public void Set(Card card, Action onClick)
    {
        gameObject.SetActive(true);
        canPlayHighlight.SetActive(false);
        _onClick = onClick;
        _card = card;
        
        nameLabel.text = _card.Name;
        description.text = _card.Description;
        type.text = _card.TypeDescription;
        art.sprite = _card.Art;
        
        var cost = card.Cost;
        costLabel.text = cost.Cost.ToString();
        costResourceTypeIcon.sprite = cost.ResourceType.Icon;
        costPanel.SetActive(!cost.ResourceType.Name.Equals("None") && cost.Cost > 0);
        
        tint.color = classTints.TintFor(card.LimitedToClass.OrDefault(() => ""));
    }

    public void SetCanPlay(bool canPlay) => canPlayHighlight.SetActive(canPlay);
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (battleState.SelectionStarted)
            return;
        _onClick();
    }

    public void SetHighlight(bool active)
    {
        if (highlight.activeSelf == active)
            return;
        
        highlight.SetActive(active);
        var sign = active ? 1 : -1;
        var scale = active ? new Vector3(highlightedScale, highlightedScale, highlightedScale) : new Vector3(1f, 1f, 1f);
        transform.localScale = scale;
        transform.localPosition += new Vector3(0, sign * 100f, sign * 2f);
    }
}
