using System.Linq;
using TMPro;
using UnityEngine;

public class StoryEventPresenter : OnMessage<ShowStoryEventResolution, ShowCreditChange, 
    ShowGainedEquipment, ShowCardReward, ShowStoryEventResultMessage>
{
    [SerializeField] private TextMeshProUGUI storyTextArea;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private TextCommandButton optionButtonPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private GameObject rewardParent;
    [SerializeField] private StoryCreditsRewardPresenter creditsPrototype;
    [SerializeField] private StoryEquipmentRewardPresenter equipmentPrototype;
    [SerializeField] private StoryCardRewardPresenter cardPrototype;
    [SerializeField] private StoryEventResultTextPresenter textPrototype;
    [SerializeField] private AdventureProgress2 adventure;
    
    private TextCommandButton[] _buttons;

    public void Present(StoryEvent s)
    {
        Message.Publish(new HideDieRoll());
        rewardParent.DestroyAllChildren();
        InitFreshOptionsButtons();
        var ctx = new StoryEventContext(adventure.CurrentChapterNumber, adventure.CurrentChapter.RewardRarityFactors, party, allEquipmentPool);
        storyTextArea.text = s.StoryText;
        for (var i = _buttons.Length - 1; i > -1; i--)
        {
            if (s.Choices.Length <= i)
            {
                _buttons[i].Hide();
                continue;
            }

            var choice = s.Choices[i];
            var choiceText = choice.ChoiceFullText(ctx);
            _buttons[i].Init(choiceText, () => choice.Select(ctx));
            if (!choice.CanSelect(ctx))
            {
                Debug.Log($"Story Event: Cannot choose {choiceText}. Condition not met.");
                _buttons[i].SetButtonDisabled(true, Color.white);
            }
        }
    }

    private void ClearOptions() => optionsParent.DestroyAllChildren();
    private void InitFreshOptionsButtons()
    {
        ClearOptions();
        // Necessary due to TextMeshProUGIU Vertex Color
        _buttons = Enumerable.Range(0, 4)
            .Select(x => Instantiate(optionButtonPrototype, optionsParent.transform))
            .ToArray();
    }
    
    protected override void Execute(ShowStoryEventResolution msg)
    {        
        InitFreshOptionsButtons();
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

    protected override void Execute(ShowStoryEventResultMessage msg)
    {
        rewardParent.DestroyAllChildren();
        Instantiate(textPrototype, rewardParent.transform).Init(msg.Text);
    }
}
