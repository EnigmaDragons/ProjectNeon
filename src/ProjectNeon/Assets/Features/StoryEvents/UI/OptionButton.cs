using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    private StoryEventChoice2 _choice;
    
    public void Init(StoryEventChoice2 choice, StoryEventContext ctx)
    {
        _choice = choice;
        text.text = choice.ChoiceFullText(ctx);
        button.onClick.AddListener(() => choice.Select(ctx));
    }

    public void Init(string choice, Action action)
    {
        text.text = choice;
        button.onClick.AddListener(() => action());
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _choice?.Reward?.Preview();
        _choice?.Penalty?.Preview();
    }

    public void OnPointerExit(PointerEventData eventData) 
        => Message.Publish(new HideStoryEventPreviews());
}