using System;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using UnityEngine.EventSystems;

public class AdventureDisplayPresenter : MonoBehaviour, ILocalizeTerms, IPointerClickHandler
{
    [SerializeField] private Image image;
    [SerializeField] private Localize nameText;
    [SerializeField] private Localize storyText;
    [SerializeField] private Button selectButton;
    
    [SerializeField] private Image[] heroBusts;
    [SerializeField] private GameObject[] heroIcons;
    [SerializeField] private GameObject allHeroesText;
    [SerializeField] private Localize lengthText;
    [SerializeField] private Localize heroLimitDescriptionLabel;
    [SerializeField] private GameObject lockVisual;
    [SerializeField] private Localize lockReasonLabel;
    [SerializeField] private Image hoverGlow;
    [SerializeField] private GameObject isCompletedView;
    [SerializeField] private BossSelectionPresenter bossSelectionPresenter;
    [SerializeField] private CurrentBoss boss;
    [SerializeField] private ActionComponent changeBoss;
    [SerializeField] private SelectableComponent component;
    
    private const string ChaptersTerm = "Menu/Chapters";
    
    public void Init(Adventure adventure, Action onSelect)
    {
        image.sprite = adventure.AdventureImage;
        nameText.SetTerm(adventure.TitleTerm);
        storyText.SetTerm(adventure.StoryTerm);
        storyText.gameObject.SetActive(!adventure.BossSelection);
        bossSelectionPresenter.gameObject.SetActive(adventure.BossSelection);
        changeBoss.Bind(() => bossSelectionPresenter.Next());
        lengthText.SetTerm(ChaptersTerm.ToLocalized().SafeFormatWithDefault("{0} Chapters", adventure.DynamicStages.Length));
        DisplayHeroPool(adventure);
        selectButton.onClick.AddListener(() =>
        {
            boss.Boss = bossSelectionPresenter.SelectedBoss;
            onSelect();
        });
        selectButton.enabled = !adventure.IsLocked;
        lockVisual.SetActive(adventure.IsLocked);
        if (adventure.IsLocked)
            Destroy(component);
        lockReasonLabel.SetFinalText(adventure.LockConditionExplanation);
        isCompletedView.SetActive(!adventure.IsLocked && adventure.IsCompleted);
        if (adventure.IsLocked)
            hoverGlow.color = new Color(0, 0, 0, 0);
    }

    private void DisplayHeroPool(Adventure adventure)
    {
        var hasDescription = !string.IsNullOrWhiteSpace(adventure.AllowedHeroesDescriptionTerm.ToEnglish());
        allHeroesText.SetActive(adventure.RequiredHeroes.Length == 0 && !hasDescription);

        if (hasDescription)
        {
            heroLimitDescriptionLabel.SetTerm(adventure.AllowedHeroesDescriptionTerm);
            heroLimitDescriptionLabel.gameObject.SetActive(true);
            heroIcons.ForEach(h => h.SetActive(false));
        }
        else
        {
            heroLimitDescriptionLabel.gameObject.SetActive(false);
            heroIcons[0].SetActive(adventure.RequiredHeroes.Length > 0);
            heroIcons[1].SetActive(adventure.RequiredHeroes.Length > 1);
            heroIcons[2].SetActive(adventure.RequiredHeroes.Length > 2);
            if (adventure.RequiredHeroes.Length > 0)
                heroBusts[0].sprite = adventure.RequiredHeroes[0].Bust;
            if (adventure.RequiredHeroes.Length > 1)
                heroBusts[1].sprite = adventure.RequiredHeroes[1].Bust;
            if (adventure.RequiredHeroes.Length > 2)
                heroBusts[2].sprite = adventure.RequiredHeroes[2].Bust;
        }
    }

    public string[] GetLocalizeTerms()
        => new[]
        {
            ChaptersTerm
        };

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            bossSelectionPresenter.Next();
    }
}
