using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/TutorialSlideshow", fileName = "Tutorial-")]
public class TutorialSlideshow : ScriptableObject, ILocalizeTerms
{
    [SerializeField] private string tutorialName;
    [SerializeField] public string displayName;
    [SerializeField] private GameObject backgroundPrototype;
    [SerializeField] private SlideTextPresenterBase slideTextPresenterPrototype;
    [SerializeField] private List<TutorialSlide> slides;

    public string TutorialName => tutorialName;
    public GameObject BackgroundPrototype => backgroundPrototype;
    public SlideTextPresenterBase SlideTextPresenterPrototype => slideTextPresenterPrototype;
    public TutorialSlide[] Slides => slides.ToArray();

    public string DisplayNameTerm => string.IsNullOrWhiteSpace(displayName)
        ? "Empty"
        : $"TutorialSlides/Tutorial_DisplayName_{tutorialName}";

    public string[] GetLocalizeTerms() => new[] { DisplayNameTerm };
}
