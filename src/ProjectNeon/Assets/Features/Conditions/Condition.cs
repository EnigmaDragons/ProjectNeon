public interface Condition
{
    IPayloadProvider Resolve(CardActionContext ctx);
}
