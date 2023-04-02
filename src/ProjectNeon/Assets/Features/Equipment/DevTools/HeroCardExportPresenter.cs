using System;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HeroCardExportPresenter : MonoBehaviour
{
    [SerializeField] private BaseHero currentHero;
    [SerializeField] private Image heroBody;
    [SerializeField] private Localize heroName;
    [SerializeField] private Localize heroClassName;
    [SerializeField] private TextMeshProUGUI heroDescription;
    [SerializeField] private Localize arch1;
    [SerializeField] private Localize arch2;
    [SerializeField] private Image tintTarget;
    // [FormerlySerializedAs("roleDescription")] [SerializeField] private Localize complexityLabel;
    // [SerializeField] private Slider complexitySlider;
    // [SerializeField] private Localize backstory;
    // [SerializeField] private NamedGameObject[] tabTargets;
    [SerializeField] private ResourceCounterPresenter resource1;
    [SerializeField] private ResourceCounterPresenter resource2;
    // [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI startingCredits;
    [SerializeField] private MemberSimplifiedVisualStatPanel statPanel;
    // [SerializeField] private SimpleDeckUI deckUi;
    // [SerializeField] private SimpleDeckUI basicUi;

    private bool _isInitialized;
    
    private void Start()
    {
        if (!_isInitialized && currentHero != null)
            Init(currentHero);
    }

    public void Hide() => gameObject.SetActive(false);
    
    public void Init(BaseHero c) => Init(c, c.AsMemberForLibrary());
    public void Init(BaseHero c, Member m)
    {
        _isInitialized = true;
        currentHero = c;
        heroBody.sprite = c.BodySprite;
        heroName.SetTerm(c.NameTerm());
        heroClassName.SetTerm(c.ClassTerm());
        heroDescription.text = c.DescriptionTerm().ToEnglish();
        tintTarget.color = c.Tint.WithAlpha(tintTarget.color.a);
        // heroDescription.SetTerm(c.DescriptionTerm());
        arch1.SetFinalText(c.Archetypes.ToArray()[0]);
        arch2.SetFinalText(c.Archetypes.ToArray()[1]);
        // complexityLabel.SetFinalText($"{"Menu/Complexity".ToLocalized()}:");
        // complexitySlider.value = Mathf.Clamp(c.ComplexityRating / 5f, 0.2f, 1f);
        // backstory.SetTerm(c.BackStoryTerm());
        var member = m;
        if (statPanel != null)
        {
            statPanel.Init(member);
            var resourceGains = c.TurnResourceGains;
            if (resourceGains.Length > 0)
                if (resourceGains[0].BaseAmount > 0)
                    statPanel.SetPrimaryResourceGain(resourceGains[0].ResourceType.Icon, resourceGains[0].BaseAmount);
        }
        // if (deckUi != null)
        //     deckUi.Init(c.Deck.Cards.Select(card => card.ToNonBattleCard(c, c.Stats)).ToArray());
        // if (basicUi != null)
        //     basicUi.Init(c.BasicCard.ToNonBattleCard(c, c.Stats).AsArray(), false);
        if (resource1 != null)
        {
            resource1.SetReactivity(false);
            if(member.State.ResourceTypes.Length < 1)
                resource1.Hide();
            else
                resource1.Init(member, member.State.ResourceTypes[0], false);
        }
        if (resource2 != null)
        { 
            resource2.SetReactivity(false);
            if(member.State.ResourceTypes.Length < 2)
                resource2.Hide();
            else
                resource2.Init(member, member.State.ResourceTypes[1], false);
        }

        // if (startingCredits != null)
        //     startingCredits.text = c.StartingCredits.ToString();
    }
}
