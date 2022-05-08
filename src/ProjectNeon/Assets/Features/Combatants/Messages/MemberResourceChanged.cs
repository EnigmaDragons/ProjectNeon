
public class MemberResourceChanged
{
    public int MemberId { get; }
    public ResourceQuantity ResourceQuantity { get; }
    public bool WasPaidCost { get; }

    public MemberResourceChanged(int memberId, ResourceQuantity qty, bool wasPaidCost)
    {
        MemberId = memberId;
        WasPaidCost = wasPaidCost;
        ResourceQuantity = qty;
    }
}
