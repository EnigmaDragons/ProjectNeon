using System.Linq;
using TMPro;
using UnityEngine;

public class StoryEventPresenter : OnMessage<ShowStoryEventResolution, ShowCreditChange, ShowGainedEquipment, ShowCardReward>
{
    [SerializeField] private TextMeshProUGUI storyTextArea;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private TextCommandButton optionButtonPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject rewardParent;
    [SerializeField] private StoryCreditsRewardPresenter creditsPrototype;
    [SerializeField] private StoryEquipmentRewardPresenter equipmentPrototype;
    [SerializeField] private StoryCardRewardPresenter cardPrototype;
    
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
            if (!choice.CanSelect(new StoryEventContext(party)))
            {
                Debug.Log($"Story Event: Cannot choose {choice.Text}. Condition not met.");
                _buttons[i].SetButtonDisabled(true, Color.white);
            }
        }
    }

    private void ClearOptions() => optionsParent.DestroyAllChildren();
    
    protected override void Execute(ShowStoryEventResolution msg)
    {        
        storyTextArea.text = msg.Story;
        for (var i = _buttons.Length - 1; i > -1; i--)
        {
            if (i == 1)
                _buttons[i].Init("Done", () => Message.Publish(new MarkStoryEventCompleted()));
            else
                _buttons[i].Hide();
        }
    }

    protected override void Execute(ShowCreditChange msg)
    {
        rewardParent.DestroyAllChildren();
        Instantiate(creditsPrototype, rewardParent.transform).Init(msg.Amount);
    }

    protected override void Execute(ShowGainedEquipment msg)
    {
        rewardParent.DestroyAllChildren();
        Instantiate(equipmentPrototype, rewardParent.transform).Init(msg.Equipment);
    }

    protected override void Execute(ShowCardReward msg)
    {
        rewardParent.DestroyAllChildren();
        Instantiate(cardPrototype, rewardParent.transform).Init(msg.Card);
    }
}
