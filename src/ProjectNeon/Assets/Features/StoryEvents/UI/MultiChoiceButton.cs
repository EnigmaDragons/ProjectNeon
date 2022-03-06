using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiChoiceButton : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI text;

    private StoryEventChoice2 _choice;
    private StoryEventContext ctx;
    
    public void Init(StoryEventChoice2 choice, StoryEventContext ctx, StoryEvent2 owner)
    {
        if (choice.Resolution.Length != 1)
            Log.Error($"More than one resolution on a multi-choice option on event: {owner.name}");
        _choice = choice;
        text.text = choice.ChoiceFullText(ctx, owner);
        button.onClick.AddListener(() => toggle.isOn = !toggle.isOn);
    }
    
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Apply()
    {
        if (toggle.isOn)
            _choice.Resolution.First().Result.Apply(ctx);
    }
}