public class GroupExtensions
{
    public static Group Inverted(Group group)
    {
        if (group == Group.Opponent)
            return Group.Ally;
        if (group == Group.Ally)
            return Group.Opponent;
        return group;
    }
}