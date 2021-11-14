
public class MessagePublishSegment : CutsceneSegment
{
    private readonly object _startMessage;
    private readonly object _fastForwardMessage;

    public MessagePublishSegment(object startMessage)
        : this(startMessage, null) {}
        
    public MessagePublishSegment(object startMessage, object fastForwardMessage)
    {
        _startMessage = startMessage;
        _fastForwardMessage = fastForwardMessage;
    }
    
    public void Start() => Message.Publish(_startMessage);

    public void FastForwardToFinishInstantly()
    {
        if (_fastForwardMessage != null)
            Message.Publish(_fastForwardMessage);
    }
}