public class Tutorial8Orchestartor : OnMessage<CardResolutionFinished>
{
    private const string _callerId = "Tutorial8Orchestrator";

    private bool _hasShowedLine1;
    private bool _hasShowedLine2;
    
    private void Start()
    {
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.ClickableControls, false, _callerId));
        Message.Publish(new SetBattleUiElementVisibility(BattleUiElement.TrashRecycleDropArea, false, _callerId));
    }
    
    protected override void Execute(CardResolutionFinished msg)
    {
        if (msg.CardName != "Energize" && !_hasShowedLine1)
        {
            Message.Publish(new ShowHeroBattleThought(4, "Can't hit me! I dodged that without a scratch!"));
            _hasShowedLine1 = true;
        }
        else if (msg.CardName != "Energize" && !_hasShowedLine2)
        {
            Message.Publish(new ShowHeroBattleThought(4, "OW! I'm all out of dodge!"));
            _hasShowedLine2 = true;
        }
    }
}