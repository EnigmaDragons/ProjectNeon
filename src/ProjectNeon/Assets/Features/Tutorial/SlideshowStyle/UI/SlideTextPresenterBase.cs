using UnityEngine;

public abstract class SlideTextPresenterBase : MonoBehaviour
{
    [SerializeField] private GameObject nextButtonIndicator;
    [SerializeField] private GameObject previousButtonIndicator;
    [SerializeField] private GameObject doneIndicator;

    public void Init<T>(string text, IndexSelector<T> slides)
    {
        InitText(text);
        previousButtonIndicator.SetActive(!slides.IsFirstItem);
        nextButtonIndicator.SetActive(!slides.IsLastItem);
        doneIndicator.SetActive(slides.IsLastItem);
    }

    protected abstract void InitText(string text);
}