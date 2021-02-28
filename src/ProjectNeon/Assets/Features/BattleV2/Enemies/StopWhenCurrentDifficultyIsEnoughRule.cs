public class StopWhenCurrentDifficultyIsEnoughRule : EncounterBuildingRule
{
    private readonly float _flexibility;

    public StopWhenCurrentDifficultyIsEnoughRule(float flexibility)
        => _flexibility = flexibility;

    public EncounterBuildingContext Filter(EncounterBuildingContext ctx)
        => ctx.CurrentDifficulty > ctx.TargetDifficulty * (1 - _flexibility) && ctx.SelectedEnemies.Length > 0
            ? ctx.WithPossibilities(new Enemy[0])
            : ctx;
}