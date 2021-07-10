
public class InfoMemberId
{
    private static int id = int.MinValue;

    public static void Reset() => id = int.MinValue;
    public static int Get() => id++;
}