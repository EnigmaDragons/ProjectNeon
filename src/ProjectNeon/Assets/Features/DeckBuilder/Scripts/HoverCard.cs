using UnityEngine;
using UnityEngine.UI;

class HoverCard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 offset;
    [SerializeField] private CardPresenter presenter;

    private CanvasScaler _scaler;
    private bool _followMouse;

    public HoverCard Initialized(CardType card)
    {
        Init(card, true);
        return this;
    }
    
    public void Init(CardType card, bool followMouse)
    {
        presenter.Set(card, () => {});
        _followMouse = followMouse;
    }

    public void Init(Card card, bool followMouse)
    {
        presenter.Set(card, () => {});
        _followMouse = followMouse;
    }

    private void Start()
    {
        _scaler = FindObjectOfType<CanvasScaler>();
    }

    private void Update()
    {
        if (!_followMouse)
            return;
        rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x * _scaler.referenceResolution.x / Screen.width, Input.mousePosition.y * _scaler.referenceResolution.y / Screen.height);
        rectTransform.anchoredPosition = rectTransform.anchoredPosition + offset + new Vector2(0, -1080);
    }
}
