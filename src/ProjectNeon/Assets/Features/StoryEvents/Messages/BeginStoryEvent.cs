
public class BeginStoryEvent
{
    public StoryEvent StoryEvent { get; }

    public BeginStoryEvent(StoryEvent e) => StoryEvent = e;
}
