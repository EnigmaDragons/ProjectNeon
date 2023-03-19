using System;

public static class ExceptionSuppressor
{
    private const string actionMsgFormat = "Exception occured during {0}";
    
    public static void LogAndContinue(Action action, string actionDescription)
    {
        try
        {
            action();
        }
        catch (Exception e)
        {
            var newEx = new Exception(string.Format(actionMsgFormat, actionDescription), e);
            Log.Error(e);
        }
    }
}
