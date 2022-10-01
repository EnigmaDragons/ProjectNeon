
using System;

public class ShowPrefactoredReward
{
    public int NumCredits { get; }
    public int NumClinicVouchers { get; }
    public Action OnProceed { get; }

    public ShowPrefactoredReward(int numCredits, int numClinicVouchers, Action onProceed)
    {
        NumCredits = numCredits;
        NumClinicVouchers = numClinicVouchers;
        OnProceed = onProceed;
    }
}
