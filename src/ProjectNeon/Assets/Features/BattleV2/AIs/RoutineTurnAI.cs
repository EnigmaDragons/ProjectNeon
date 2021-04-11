using System.Collections.Generic;
using System.Linq;

public abstract class RoutineTurnAI : TurnAI
{
    private readonly Dictionary<int, Queue<string>> _currentRoutineMap = new Dictionary<int, Queue<string>>();

    public sealed override IPlayedCard Play(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var anticipatedCard = Anticipate(memberId, battleState, strategy);
        if (_currentRoutineMap[memberId].Any())
            _currentRoutineMap[memberId].Dequeue();
        return anticipatedCard;
    }

    public override IPlayedCard Anticipate(int memberId, BattleState battleState, AIStrategy strategy)
    {
        var ctx = new CardSelectionContext(memberId, battleState, strategy);
        if (!ctx.CardOptions.Any())
            return ctx.WithSelectedTargetsPlayedCard();
        if (_currentRoutineMap.ContainsKey(memberId))
        {
            while (_currentRoutineMap[memberId].Any() && !ctx.CardOptions.Any(x => x.Name.Equals(_currentRoutineMap[memberId].Peek())))
                _currentRoutineMap[memberId].Dequeue();
            if (!_currentRoutineMap[memberId].Any())
                _currentRoutineMap[memberId] = ChooseRoutine(ctx);
        }
        else
        {
            _currentRoutineMap[memberId] = ChooseRoutine(ctx);
        }
        while (_currentRoutineMap[memberId].Any() && !ctx.CardOptions.Any(x => x.Name.Equals(_currentRoutineMap[memberId].Peek())))
            _currentRoutineMap[memberId].Dequeue();
        if (!_currentRoutineMap[memberId].Any())
            Log.Error($"{this.GetType().Name} couldn't supply a routine that had an available action, when there was card options to choose");
        var playedCard = ctx
            .WithSelectedCardByNameIfPresent(_currentRoutineMap[memberId].Peek())
            .WithSelectedTargetsPlayedCard();
        return playedCard;
    }

    protected abstract Queue<string> ChooseRoutine(CardSelectionContext ctx);
}