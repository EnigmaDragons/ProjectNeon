using UnityEngine;
using UnityEngine.UI;

class HoverCard : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 offset;
    [SerializeField] private CardPresenter presenter;

    private CanvasScaler _scaler;

    public void Init(Card card)
    {
        presenter.Set(card, () => {});
    }

    private void Start()
    {
        _scaler = FindObjectOfType<CanvasScaler>();
    }

    private void Update()
    {
        rectTransform.anchoredPosition = new Vector2(Input.mousePosition.x * _scaler.referenceResolution.x / Screen.width, Input.mousePosition.y * _scaler.referenceResolution.y / Screen.height);
        rectTransform.anchoredPosition = rectTransform.anchoredPosition + offset + new Vector2(0, -1080);
    }
}
