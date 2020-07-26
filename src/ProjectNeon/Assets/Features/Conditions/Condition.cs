public interface Condition
{
    IPayloadProvider Resolve(Member source, Target target, Group group, Scope scope, int amountPaid);
}