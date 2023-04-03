using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MultiUnlockPresenter : OnMessage<DismissUnlockDisplay>
{
    [SerializeField] private GameObject viewParent;
    [SerializeField] private Graphic arrowsAnim;
    [SerializeField] private Image darken;
    [SerializeField] private UnlockPresenter presenter1;
    [SerializeField] private UnlockPresenter presenter2;
    [SerializeField] private UnlockPresenter presenter3;
    [SerializeField] private float positionOneForOne;
    [SerializeField] private float positionOneForTwo;
    [SerializeField] private float positionTwoForTwo;
    [SerializeField] private float positionOneForThree;
    [SerializeField] private float positionTwoForThree;
    [SerializeField] private float positionThreeForThree;

    private float _duration = 0.4f;
    private float _darkenAlpha;
        
    private void Awake()
    {
        _darkenAlpha = darken.color.a;
    }
    
    public void Show(UnlockUiData[] u)
    {
        if (u.Length == 0)
            return;
        else if (u.Length == 1)
        {
            presenter1.Show(u[0], positionOneForOne);
            presenter2.Hide();
            presenter3.Hide();
        }
        else if (u.Length == 2)
        {
            presenter1.Show(u[0], positionOneForTwo);
            presenter2.Show(u[1], positionTwoForTwo);
            presenter3.Hide();
        }
        else if (u.Length >= 3)
        {
            presenter1.Show(u[0], positionOneForThree);
            presenter2.Show(u[1], positionTwoForThree);
            presenter3.Show(u[2], positionThreeForThree);
        }
        
        arrowsAnim.color = Color.white.Transparent();
        viewParent.SetActive(true);
        darken.color = new Color(darken.color.r, darken.color.g, darken.color.b, 0);
        darken.DOFade(_darkenAlpha, _duration);
        arrowsAnim.DOColor(Color.white.WithAlpha(0.1f), 2).SetEase(Ease.InOutQuad);
    }
        
    public void Hide() => viewParent.SetActive(false);

    protected override void Execute(DismissUnlockDisplay msg) => Hide();
}
