
public static class NextCardId
{
    private static int id;

    public static void Reset() => id = 0;
    public static int Get() => id++;
}
