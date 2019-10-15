public class ManaDrain : TurnStats
{
    private int quantity = 10;
    public override int Magic => this.Origin.Magic + quantity;
}
