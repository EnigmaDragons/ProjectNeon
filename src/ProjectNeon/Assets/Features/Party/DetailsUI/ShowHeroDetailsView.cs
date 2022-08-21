
public class ShowHeroDetailsView
{
    public Hero Hero { get; }
    public Maybe<Member> Member { get; }

    public ShowHeroDetailsView(Hero h, Maybe<Member> member)
    {
        Hero = h;
        Member = member;
    }
}
