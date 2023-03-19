
public class DebugLocalization
{
    public static bool Value = false;

    public static void Write(string msg)
    {
        if (Value)
            Log.Info(msg);
    }
}