
public class ShowCreditChange
{
    public int Amount { get; }
    public bool IsReward => Amount > 0;

    public ShowCreditChange(int amount) => Amount = amount;
}
