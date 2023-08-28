using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/Slide", fileName = "Tutorial-")]
public class TutorialSlide : ScriptableObject, ILocalizeTerms
{
    [SerializeField] public int id;
    [SerializeField, TextArea(4, 6)] public string text;
    [SerializeField] private GameObject uiElementPrototype;

    public string Text => Term;
    public string Term => InputControl.Type == ControlType.Mouse || InputControl.Type == ControlType.Keyboard ? $"TutorialSlides/Tutorial_Slide_{id}" : $"TutorialSlides/Tutorial_Slide_{id}_Controller";
    public GameObject UiElementPrototype => uiElementPrototype;

    public string[] GetLocalizeTerms() => new [] { $"TutorialSlides/Tutorial_Slide_{id}", $"TutorialSlides/Tutorial_Slide_{id}_Controller" };
}
