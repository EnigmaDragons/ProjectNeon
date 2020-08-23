public class DrawCardsOverTime : Effect
{
    private readonly PlayerState _playerState;
    private readonly int _amount;
    private readonly int _turns;

    public DrawCardsOverTime(PlayerState playerState, int amount, int turns)
    {
        _playerState = playerState;
        _amount = amount;
        _turns = turns;
    }
    
    public void Apply(Member source, Target target)
    {
        _playerState.AddState(new AdjustedPlayerStats(new PlayerStatAddends().With(PlayerStatType.CardDraw, _amount), _turns, _amount < 0, _turns < 0));
    }
}