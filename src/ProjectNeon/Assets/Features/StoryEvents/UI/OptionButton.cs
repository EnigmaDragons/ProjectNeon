using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Localize localize;

    private StoryEventChoice2 _choice;
    
    public void Init(StoryEventChoice2 choice, StoryEventContext ctx, StoryEvent2 owner)
    {
        _choice = choice;
        localize.SetTerm(choice.Term);
        button.onClick.AddListener(() => choice.Select(ctx, owner, Input.GetKey(KeyCode.KeypadPlus) ? 0 : Input.GetKey(KeyCode.KeypadMinus) ? 0.99 : Maybe<double>.Missing()));
    }

    public void Init(string choice, Action action)
    {
        localize.SetTerm(choice);
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