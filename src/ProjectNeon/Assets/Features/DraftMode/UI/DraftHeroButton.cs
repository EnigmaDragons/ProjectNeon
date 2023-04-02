using UnityEngine;
using UnityEngine.UI;

public class DraftHeroButton : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private GameObject selected;

    public void Init(BaseHero h, bool isSelected)
    {
        heroBust.sprite = h.Bust;
        gameObject.SetActive(true);
        selected.SetActive(isSelected);
    }

    public void Disable() => gameObject.SetActive(false);
    public void Select() => selected.SetActive(true);
    public void Unselect() => selected.SetActive(false);
}
