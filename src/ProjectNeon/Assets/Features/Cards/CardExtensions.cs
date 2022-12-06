
public static class CardExtensions
{
    public static void ShowDetailedCardView(this Card c) => Message.Publish(new ShowDetailedCardView(c));
    
    public static bool RequiresPlayerTargeting(this CardTypeData c) 
        => c.ActionSequences.AnyNonAlloc(x 
            => x.Group != Group.Self
                && x.Scope != Scope.All
                && x.Scope != Scope.AllExceptSelf
                && x.Scope != Scope.Random
                && x.Scope != Scope.RandomExceptTarget);
}
