public class Multiple : Target
{
    private Member[] _members;

    public Multiple(Member[] members)
    {
        _members = members;
    }

    public Member[] Members => _members;
}
