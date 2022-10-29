using UnityEngine;

[CreateAssetMenu(menuName = "Tutorial/Slideshow/Slide", fileName = "Tutorial-")]
public class TutorialSlide : ScriptableObject
{
    [SerializeField, ReadOnly] public int id;
    [SerializeField, TextArea(4, 6)] private string text;
    [SerializeField] private GameObject uiElementPrototype;

    public string Text => text;
    public GameObject UiElementPrototype => uiElementPrototype;
}
