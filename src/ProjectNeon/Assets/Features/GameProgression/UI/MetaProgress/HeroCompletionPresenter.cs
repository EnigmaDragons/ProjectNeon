using UnityEngine;
using UnityEngine.UI;

public class HeroCompletionPresenter : MonoBehaviour
{
    [SerializeField] private Image heroBust;
    [SerializeField] private GameObject checkMark;

    public HeroCompletionPresenter Initialized(BaseHero h, bool isCompleted)
    {
        heroBust.sprite = h.Bust;
        checkMark.SetActive(isCompleted);
        gameObject.SetActive(true);
        return this;
    }

    public HeroCompletionPresenter Hidden()
    {
        gameObject.SetActive(false);
        return this;
    }
}
