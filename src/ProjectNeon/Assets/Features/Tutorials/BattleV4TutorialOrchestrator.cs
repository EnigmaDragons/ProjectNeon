using UnityEngine;

public class BattleV4TutorialOrchestrator : OnMessage<CardResolutionFinished, Finished<ShowTooltip>, TurnStarted, SwappedCard, ShowEnemyDetails, ShowDetailedCardView>
{
    [SerializeField] private TutorialSlideshow intro;
    [SerializeField] private TutorialSlideshow afterShieldPlay;
    [SerializeField] private TutorialSlideshow afterStatusLook;
    [SerializeField] private TutorialSlideshow afterAttackPlay;
    [SerializeField] private TutorialSlideshow startOfTurn2;
    [SerializeField] private TutorialSlideshow afterInspectingEnemy;
    [SerializeField] private TutorialSlideshow afterInspectingCard;
    [SerializeField] private TutorialSlideshow startOfTurn3;
    [SerializeField] private TutorialSlideshow startOfTurn4;
    [SerializeField] private TutorialSlideshow afterLookingAtBasic;
    [SerializeField] private TutorialSlideshow afterGaining6Resources;

    [SerializeField] private BattleState state;
    
    private bool _hasPlayedTutorialAfterShieldPlay;
    private bool _hasPlayedTutorialAfterStatusLook;
    private bool _hasPlayedTutorialAfterAttackPlay;
    private int _turnNumber;
    private bool _hasInspectedAnEnemy;
    private bool _hasInspectedACard;
    private bool _hasToggledCardAsBasic;
    private bool _hasPlayedTutorialAfterGaining6Resources;

    protected override void Execute(CardResolutionFinished msg)
    {
        if (!_hasPlayedTutorialAfterShieldPlay && state.Heroes[0].CurrentShield() > 0)
        {
            _hasPlayedTutorialAfterShieldPlay = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterShieldPlay));
        }
        else if (!_hasPlayedTutorialAfterAttackPlay && state.Enemies[0].Member.CurrentShield() < 10)
        {
            _hasPlayedTutorialAfterAttackPlay = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterAttackPlay));
        }
        else if (!_hasPlayedTutorialAfterGaining6Resources && state.Heroes[0].PrimaryResourceQuantity().Amount >= 6)
        {
            _hasPlayedTutorialAfterGaining6Resources = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterGaining6Resources));
        }
    }

    protected override void Execute(Finished<ShowTooltip> msg)
    {
        if (!_hasPlayedTutorialAfterStatusLook && msg.Message.Text.Contains("Glitch"))
        {
            _hasPlayedTutorialAfterStatusLook = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterStatusLook));
        }
    }

    protected override void Execute(TurnStarted msg)
    {
        _turnNumber++;
        if (_turnNumber == 1)
            Message.Publish(new ShowTutorialSlideshowIfNeeded(intro));
        else if (_turnNumber == 2)
            Message.Publish(new ShowTutorialSlideshowIfNeeded(startOfTurn2));
        else if (_turnNumber == 3)
            Message.Publish(new ShowTutorialSlideshowIfNeeded(startOfTurn3));
        else if (_turnNumber == 4)
            Message.Publish(new ShowTutorialSlideshowIfNeeded(startOfTurn4));
    }

    protected override void Execute(SwappedCard msg)
    {
        if (!_hasToggledCardAsBasic)
        {
            _hasToggledCardAsBasic = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterLookingAtBasic));
        }
    }

    protected override void Execute(ShowEnemyDetails msg)
    {
        if (!_hasInspectedAnEnemy)
        {
            _hasInspectedAnEnemy = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterInspectingEnemy));
        }
    }

    protected override void Execute(ShowDetailedCardView msg)
    {
        if (!_hasInspectedACard)
        {
            _hasInspectedACard = true;
            Message.Publish(new ShowTutorialSlideshowIfNeeded(afterInspectingCard));
        }
    }
}