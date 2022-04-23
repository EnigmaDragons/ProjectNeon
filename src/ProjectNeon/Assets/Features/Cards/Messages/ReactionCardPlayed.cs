
public class ReactionCardPlayed
{
    public IPlayedCard PlayedCard { get; }

    public ReactionCardPlayed(IPlayedCard c) => PlayedCard = c;
}
