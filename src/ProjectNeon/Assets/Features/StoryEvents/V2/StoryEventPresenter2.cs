using System.Linq;
using TMPro;
using UnityEngine;

public class StoryEventPresenter2 : OnMessage<ShowStoryEventResolution, ShowCreditChange, 
    ShowGainedEquipment, ShowCardReward, ShowStoryEventResultMessage, 
    ShowTextResultPreview, ShowCredResultPreview, ShowEquipmentResultPreview>
{
    [SerializeField] private TextMeshProUGUI storyTextArea;
    [SerializeField] private GameObject optionsParent;
    [SerializeField] private OptionButton optionButtonPrototype;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private EquipmentPool allEquipmentPool;
    [SerializeField] private GameObject rewardParent;
    [SerializeField] private GameObject rewardPreviewParent;
    [SerializeField] private GameObject penaltyPreviewParent;
    [SerializeField] private StoryCreditsRewardPresenter creditsPrototype;
    [SerializeField] private StoryEquipmentRewardPresenter equipmentPrototype;
    [SerializeField] private StoryCardRewardPresenter cardPrototype;
    [SerializeField] private StoryEventResultTextPresenter textPrototype;
    [SerializeField] private StoryCreditsRewardPresenter creditsPreviewPrototype;
    [SerializeField] private StoryEquipmentRewardPresenter equipmentPreviewPrototype;
    [SerializeField] private StoryCardRewardPresenter cardPreviewPrototype;
    [SerializeField] private StoryEventResultTextPresenter textPreviewPrototype;
    [SerializeField] private AdventureProgress2 adventure;
    [SerializeField] private CurrentGameMap3 map;
    
    private OptionButton[] _buttons;

    public void Present(StoryEvent2 s)
    {
        Message.Publish(new HideDieRoll());
        rewardParent.DestroyAllChildren();
        InitFreshOptionsButtons();
        var ctx = new StoryEventContext(adventure.CurrentChapterNumber, adventure.CurrentChapter.RewardRarityFactors, party, allEquipmentPool, map, adventure);
        storyTextArea.text = s.StoryText;
        for (var i = _buttons.Length - 1; i > -1; i--)
        {
            if (s.Choices.Length <= i)
            {
                _buttons[i].Hide();
                continue;
            }
            _buttons[i].Init(s.Choices[i], ctx);
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
        rewardPreviewParent.SetActive(false);
        penaltyPreviewParent.SetActive(false);
        rewardParent.DestroyAllChildren();
        Instantiate(creditsPrototype, rewardParent.transform).Init(msg.Amount);
    }

    protected override void Execute(ShowGainedEquipment msg)
    {
        rewardPreviewParent.SetActive(false);
        penaltyPreviewParent.SetActive(false);
        rewardParent.DestroyAllChildren();
        Instantiate(equipmentPrototype, rewardParent.transform).Init(msg.Equipment);
    }

    protected override void Execute(ShowCardReward msg)
    {
        rewardPreviewParent.SetActive(false);
        penaltyPreviewParent.SetActive(false);
        rewardParent.DestroyAllChildren();
        Instantiate(cardPrototype, rewardParent.transform).Init(msg.Card);
    }

    protected override void Execute(ShowStoryEventResultMessage msg)
    {
        rewardPreviewParent.SetActive(false);
        penaltyPreviewParent.SetActive(false);
        rewardParent.DestroyAllChildren();
        Instantiate(textPrototype, rewardParent.transform).Init(msg.Text);
    }

    protected override void Execute(ShowTextResultPreview msg)
    {
        rewardPreviewParent.SetActive(true);
        penaltyPreviewParent.SetActive(true);
        var parent = msg.IsReward ? rewardPreviewParent : penaltyPreviewParent;
        parent.DestroyAllChildren();
        Instantiate(textPreviewPrototype, parent.transform).Init(msg.Text);
    }

    protected override void Execute(ShowCredResultPreview msg)
    {
        rewardPreviewParent.SetActive(true);
        penaltyPreviewParent.SetActive(true);
        var parent = msg.IsReward ? rewardPreviewParent : penaltyPreviewParent;
        parent.DestroyAllChildren();
        Instantiate(creditsPreviewPrototype, parent.transform).Init(msg.Creds);
    }

    protected override void Execute(ShowEquipmentResultPreview msg)
    {
        rewardPreviewParent.SetActive(true);
        penaltyPreviewParent.SetActive(true);
        rewardPreviewParent.DestroyAllChildren();
        Instantiate(equipmentPreviewPrototype, rewardPreviewParent.transform).Init(msg.Equipment);
    }
}
