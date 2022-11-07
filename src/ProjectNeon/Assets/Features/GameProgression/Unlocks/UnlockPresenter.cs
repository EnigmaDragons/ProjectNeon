using System;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPresenter : OnMessage<DismissUnlockDisplay>
{
    [SerializeField] private GameObject viewParent;
    [SerializeField] private Image darken;
    [SerializeField] private Graphic arrowsAnim;
    [SerializeField] private GameObject itemParent;
    [SerializeField] private Localize headerLabel;
    [SerializeField] private Localize itemLabel;
    [SerializeField] private Image itemImage;

    private float _duration = 0.4f;
    private Vector3 _targetScale;
    private float _darkenAlpha;
    private Action _onFinished = () => { };
    
    public bool IsShowing { get; private set; }
    
    private void Awake()
    {
        _darkenAlpha = darken.color.a;
        _targetScale = itemParent.transform.localScale;
        viewParent.SetActive(false);
    }

    public void Show(UnlockUiData u, Action onFinished)
    {
        headerLabel.SetTerm(u.HeaderTerm);
        itemLabel.SetTerm(u.TitleTerm);
        itemImage.sprite = u.Image;
        _onFinished = onFinished;
        arrowsAnim.color = Color.white.Transparent();

        viewParent.SetActive(true);
        darken.color = new Color(darken.color.r, darken.color.g, darken.color.b, 0);
        darken.DOFade(_darkenAlpha, _duration);
        itemParent.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        itemParent.transform.DOScale(_targetScale, _duration);
        arrowsAnim.DOColor(Color.white.WithAlpha(0.1f), 2).SetEase(Ease.InOutQuad);
        IsShowing = true;
    }

    public void Hide()
    {
        viewParent.SetActive(false);
        IsShowing = false;
        _onFinished();
    }

    protected override void Execute(DismissUnlockDisplay msg) => Hide();
}
