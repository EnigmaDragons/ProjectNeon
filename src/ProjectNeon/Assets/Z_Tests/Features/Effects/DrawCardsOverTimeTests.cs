using NUnit.Framework;

public class DrawCardsOverTimeTests
{
    [Test]
    public void DrawCardsOverTime_Apply_PlayerStatsAdjusted()
    {
        var target = TestMembers.Any();
        var playerState = new PlayerState();
        var draw = playerState.CurrentStats.CardDraw();
        
        new DrawCardsOverTime(playerState, 1, 1).Apply(target, new Single(target));

        Assert.AreEqual(draw + 1, playerState.CurrentStats.CardDraw());
    }
}