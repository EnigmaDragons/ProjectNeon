using System.Linq;

public static class CardExtensions
{
    public static void ShowDetailedCardView(this Card c) => Message.Publish(new ShowDetailedCardView(c));
    
    public static bool RequiresPlayerTargeting(this Card c) => c.ActionSequences.Any(x 
            => x.Group != Group.Self
            && x.Scope != Scope.All
            && x.Scope != Scope.AllExceptSelf);
}
