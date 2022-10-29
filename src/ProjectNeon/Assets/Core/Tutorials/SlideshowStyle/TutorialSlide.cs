using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/Slide", fileName = "Tutorial-")]
public class TutorialSlide : ScriptableObject
{
    [SerializeField] public int id;
    [SerializeField, TextArea(4, 6)] public string text;
    [SerializeField] private GameObject uiElementPrototype;

    public string Text => Term;
    public string Term => $"TutorialSlides/Tutorial_Slide_{id}";
    public GameObject UiElementPrototype => uiElementPrototype;
}
