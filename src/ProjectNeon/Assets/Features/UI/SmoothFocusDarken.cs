using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SmoothFocusDarken : MonoBehaviour
{
    [SerializeField] private Image fullScreen;
    [SerializeField] private Color finalDarkenColor = Color.black.WithAlpha(0.5f);
    [SerializeField] private float transitionDuration = 0.6f;
    
    private static readonly Color Transparent = new Color(0, 0, 0, 0);

    private void Awake() => fullScreen.color = Transparent;

    public void Show() => Go(true);
    public void Hide() => Go(false);

    private void Go(bool darkenActive)
    {
        fullScreen.DOKill();
        fullScreen.DOColor(darkenActive ? finalDarkenColor : Transparent, transitionDuration);
    }
}
