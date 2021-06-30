using UnityEngine;
using UnityEngine.UI;

public class CardBustPresenter : MonoBehaviour
{
    [SerializeField] private Image image;

    public void Show(Sprite s)
    {
        gameObject.SetActive(true);
        image.sprite = s;
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}