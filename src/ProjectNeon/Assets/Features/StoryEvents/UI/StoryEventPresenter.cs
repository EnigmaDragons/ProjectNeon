using System.Linq;
using TMPro;
using UnityEngine;

public class StoryEventPresenter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storyTextArea;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private TextCommandButton optionButtonPrototype;
    [SerializeField] private PartyAdventureState party;

    private TextCommandButton[] _buttons;
    
    private void Awake()
    {
        ClearOptions();
        _buttons = Enumerable.Range(0, 4)
            .Select(x => Instantiate(optionButtonPrototype, optionsParent.transform))
            .ToArray();
    }
    
    public void Present(StoryEvent s)
    {
        storyTextArea.text = s.StoryText;
        for (var i = _buttons.Length - 1; i > -1; i--)
        {
            if (s.Choices.Length <= i)
            {
                _buttons[i].Hide();
                continue;
            }

            var choice = s.Choices[i];
            _buttons[i].Init(choice.Text, () => choice.Select(new StoryEventContext(party)));
            
        }
    }

    private void ClearOptions() => optionsParent.DestroyAllChildren();
}
