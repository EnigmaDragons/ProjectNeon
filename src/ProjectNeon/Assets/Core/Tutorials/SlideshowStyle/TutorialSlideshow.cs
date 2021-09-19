using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/TutorialSlideshow")]
public class TutorialSlideshow : ScriptableObject
{
    [SerializeField] private string tutorialName;
    [SerializeField] private List<TutorialSlide> slides;

    public string TutorialName => tutorialName;
    public TutorialSlide[] Slides => slides.ToArray();
}
