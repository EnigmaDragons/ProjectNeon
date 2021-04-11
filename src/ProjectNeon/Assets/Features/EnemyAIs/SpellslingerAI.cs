using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Spellslinger")]
public class SpellslingerAI : RoutineTurnAI
{
    private Queue<string>[] NormalRoutines => new Queue<string>[]
    {
        new Queue<string>(new [] { "Spark", "BOOM!" }),
        new Queue<string>(new [] { "Omen", "Curse" }),
    };
    private Queue<string> UltimateRoutine => new Queue<string>(new[] {"Calm", "Storm"});

    protected override Queue<string> ChooseRoutine(CardSelectionContext ctx)
        => ctx.CardOptions.Any(x => x.Tags.Contains(CardTag.Ultimate))
            ? UltimateRoutine
            : NormalRoutines.Random();
}