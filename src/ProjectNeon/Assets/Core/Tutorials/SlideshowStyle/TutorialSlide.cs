using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/Slide")]
public class TutorialSlide : ScriptableObject
{
    [SerializeField, TextArea(4, 6)] private string text;
    [SerializeField] private GameObject uiElementPrototype;

    public string Text => text;
    public GameObject UiElementPrototype => uiElementPrototype;
}
