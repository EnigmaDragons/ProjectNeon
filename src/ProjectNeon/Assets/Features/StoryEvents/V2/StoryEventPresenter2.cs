using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryEventPresenter2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storyNameLabel;
    [SerializeField] private TextMeshProUGUI storyTextArea;
    [SerializeField] private Image corpLogo;
    [SerializeField] private UnityEngine.UI.Extensions.Gradient corpTint;
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
    [SerializeField] private FloatReference outcomeDelay;
    [SerializeField] private GameObject rewardPreviewLabel;
    [SerializeField] private GameObject penaltyPreviewLabel;
    
    private OptionButton[] _buttons;

    private void Awake()
    {
        HideOutcomesAndPreviews();
    }

    private void OnEnable()
    {
        Message.Subscribe<ShowStoryEventResolution>(Execute, this);
        Message.Subscribe<ShowCreditChange>(Execute, this);
        Message.Subscribe<ShowGainedEquipment>(Execute, this);
        Message.Subscribe<ShowCardReward>(Execute, this);
        Message.Subscribe<ShowStoryEventResultMessage>(Execute, this);
        Message.Subscribe<ShowTextResultPreview>(Execute, this);
        Message.Subscribe<ShowCredResultPreview>(Execute, this);
        Message.Subscribe<ShowEquipmentResultPreview>(Execute, this);
        Message.Subscribe<HideStoryEventPreviews>(Execute, this);
        Message.Subscribe<ShowCardResultPreview>(Execute, this);
    }

    private void OnDisable() => Message.Unsubscribe(this);

    public void Present(StoryEvent2 s)
    {
        Message.Publish(new HideDieRoll());
        rewardParent.DestroyAllChildren();
        InitFreshOptionsButtons();
        var ctx = new StoryEventContext(adventure.CurrentChapterNumber, adventure.CurrentChapter.RewardRarityFactors, party, allEquipmentPool, map, adventure);
        corpLogo.sprite = s.Corp.Logo;
        corpTint.Vertex1 = s.Corp.Color1;
        corpTint.Vertex2 = s.Corp.Color2;
        storyNameLabel.text = s.DisplayName;
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
        // Necessary due to TextMeshProUGUI Vertex Color
        _buttons = Enumerable.Range(0, 4)
            .Select(x => Instantiate(optionButtonPrototype, optionsParent.transform))
            .ToArray();
    }
    
    protected void Execute(ShowStoryEventResolution msg)
    {
        ClearStoryElements();
        this.ExecuteAfterDelay(outcomeDelay, () =>
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
        });
    }

    private void ClearStoryElements()
    {
        ClearOptions();
        storyTextArea.text = String.Empty;
    }

    private void HideOutcomesAndPreviews()
    {
        rewardParent.DestroyAllChildren();
        HidePreviews();
    }

    private void HidePreviews()
    {
        rewardPreviewLabel.SetActive(false);
        rewardPreviewParent.SetActive(false);
        rewardPreviewParent.DestroyAllChildren();
        penaltyPreviewLabel.SetActive(false);
        penaltyPreviewParent.SetActive(false);
        penaltyPreviewParent.DestroyAllChildren();
    }

    protected void Execute(ShowCreditChange msg)
    {
        HideOutcomesAndPreviews();
        this.ExecuteAfterDelay(outcomeDelay, () => Instantiate(creditsPrototype, rewardParent.transform).Init(msg.Amount));
    }

    protected void Execute(ShowGainedEquipment msg)
    {
        HideOutcomesAndPreviews();
        this.ExecuteAfterDelay(outcomeDelay, () => Instantiate(equipmentPrototype, rewardParent.transform).Init(msg.Equipment));
    }

    protected void Execute(ShowCardReward msg)
    {
        HideOutcomesAndPreviews();
        this.ExecuteAfterDelay(outcomeDelay, () => Instantiate(cardPrototype, rewardParent.transform).Init(msg.Card));
    }

    protected void Execute(ShowStoryEventResultMessage msg)
    {
        HideOutcomesAndPreviews();
        this.ExecuteAfterDelay(outcomeDelay, () => Instantiate(textPrototype, rewardParent.transform).Init(msg.Text));
    }

    protected void Execute(ShowTextResultPreview msg)
    {
        EnablePreviews();
        var parent = msg.IsReward ? rewardPreviewParent : penaltyPreviewParent;
        parent.DestroyAllChildren();
        Instantiate(textPreviewPrototype, parent.transform).Init(msg.Text);
    }

    protected void Execute(ShowCredResultPreview msg)
    {
        EnablePreviews();
        var parent = msg.IsReward ? rewardPreviewParent : penaltyPreviewParent;
        parent.DestroyAllChildren();
        Instantiate(creditsPreviewPrototype, parent.transform).Init(msg.Creds);
    }

    protected void Execute(ShowEquipmentResultPreview msg)
    {
        EnablePreviews();
        rewardPreviewParent.DestroyAllChildren();
        Instantiate(equipmentPreviewPrototype, rewardPreviewParent.transform).Init(msg.Equipment);
    }
    
    private void Execute(HideStoryEventPreviews obj) => HidePreviews();

    private void Execute(ShowCardResultPreview msg)
    {
        EnablePreviews();
        rewardPreviewParent.DestroyAllChildren();
        Instantiate(cardPreviewPrototype, rewardPreviewParent.transform).Init(msg.Card);
    }

    private void EnablePreviews()
    {
        rewardPreviewLabel.SetActive(true);
        rewardPreviewParent.SetActive(true);
        penaltyPreviewLabel.SetActive(true);
        penaltyPreviewParent.SetActive(true);
    }
}
