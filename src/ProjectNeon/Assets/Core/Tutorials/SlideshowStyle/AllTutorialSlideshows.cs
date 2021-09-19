using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/All Tutorial Slideshows")]
public class AllTutorialSlideshows : ScriptableObject
{
    [SerializeField] private List<TutorialSlideshow> tutorials;

    public Maybe<TutorialSlideshow> GetByName(string tutorialName) 
        => tutorials.FirstOrMaybe(t => t.TutorialName.Equals(tutorialName));
}
