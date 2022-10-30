using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/TutorialSlideshow", fileName = "Tutorial-")]
public class TutorialSlideshow : ScriptableObject
{
    [SerializeField] private string tutorialName;
    [SerializeField] private string displayName;
    [SerializeField] private GameObject backgroundPrototype;
    [SerializeField] private SlideTextPresenterBase slideTextPresenterPrototype;
    [SerializeField] private List<TutorialSlide> slides;

    public string DisplayName => displayName ?? TutorialName;
    public string TutorialName => tutorialName;
    public GameObject BackgroundPrototype => backgroundPrototype;
    public SlideTextPresenterBase SlideTextPresenterPrototype => slideTextPresenterPrototype;
    public TutorialSlide[] Slides => slides.ToArray();
}
