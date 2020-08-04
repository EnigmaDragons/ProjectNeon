namespace Features.Effects
{
    public sealed class StunForTurns : ITemporalState
    {
        private int _remainingDuration;

        public IStats Stats => new StatAddends().With(TemporalStatType.TurnStun, _remainingDuration);
        public bool IsDebuff => true;
        public bool IsActive => _remainingDuration > 0;
        public void AdvanceTurn() => _remainingDuration--;

        public StunForTurns(float duration) => _remainingDuration = duration.CeilingInt();
    }
}
