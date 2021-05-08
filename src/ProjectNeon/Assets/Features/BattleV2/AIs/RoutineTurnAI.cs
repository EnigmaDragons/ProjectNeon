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
        
        // Init If Needed
        if (!_currentRoutineMap.ContainsKey(memberId))
            _currentRoutineMap[memberId] = ChooseRoutine(ctx);
        var queue = _currentRoutineMap[memberId];
        
        // Clear Old Routine if Not Possible
        while (queue.Any() && !ctx.CardOptions.Any(x => x.Name.Equals(queue.Peek())))
            queue.Dequeue();
        
        // Choose a New Routine if Needed
        if (!queue.Any())
            _currentRoutineMap[memberId] = ChooseRoutine(ctx);
        while (queue.Any() && !ctx.CardOptions.Any(x => x.Name.Equals(queue.Peek())))
            queue.Dequeue();
        
        // Happens if there is a bad routine, or if the Enemy has been CCed based on Card Type
        if (!_currentRoutineMap[memberId].Any())
            Log.Info($"{GetType().Name} couldn't supply a routine that had an available action, when there were card options to choose");

        var playedCard = queue.Any()
            ? ctx
                .WithSelectedCardByNameIfPresent(queue.Peek())
                .WithSelectedTargetsPlayedCard()
            : ctx.WithSelectedTargetsPlayedCard();
        return playedCard;
    }

    protected abstract Queue<string> ChooseRoutine(CardSelectionContext ctx);
}