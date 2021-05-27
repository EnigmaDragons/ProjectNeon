using System;

public interface ITemporalCardState
{
    bool IsActive { get; }
    [Obsolete] int CostAdjustment { get; }
    void OnCardPlay();
    void OnTurnStart();
    void OnTurnEnd();
}