public interface ILogicFlow
{
    IPayloadProvider Resolve(CardActionContext ctx);
}
