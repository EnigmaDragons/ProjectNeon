using DG.Tweening;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject itemParent;
    [SerializeField] private Localize headerLabel;
    [SerializeField] private Localize itemLabel;
    [SerializeField] private Image itemImage;

    private float _duration = 0.4f;
    private Vector3 _targetScale;
    
    private void Awake()
    {
        _targetScale = itemParent.transform.localScale;
    }

    public void Show(UnlockUiData u, float xPosition)
    {
        rectTransform.anchoredPosition = new Vector2(xPosition, rectTransform.anchoredPosition.y);
        itemParent.SetActive(true);
        headerLabel.SetTerm(u.HeaderTerm);
        itemLabel.SetTerm(u.TitleTerm);
        itemImage.sprite = u.Image;
        itemParent.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        itemParent.transform.DOScale(_targetScale, _duration);
    }

    public void Hide()
    {
        itemParent.SetActive(false);
    }
}
