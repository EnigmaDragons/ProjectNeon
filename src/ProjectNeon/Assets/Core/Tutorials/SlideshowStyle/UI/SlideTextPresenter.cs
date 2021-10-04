using TMPro;
using UnityEngine;

public class SlideTextPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI slideText;
    [SerializeField] private GameObject nextButtonIndicator;
    [SerializeField] private GameObject previousButtonIndicator;
    [SerializeField] private GameObject doneIndicator;

    public void Init<T>(string text, IndexSelector<T> slides)
    {
        slideText.text = text;
        previousButtonIndicator.SetActive(!slides.IsFirstItem);
        nextButtonIndicator.SetActive(!slides.IsLastItem);
        doneIndicator.SetActive(slides.IsLastItem);
    }
}