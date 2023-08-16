using System;

public class CurrentLowQualityMode
{
    public static bool IsEnabled { get; private set; }
    
    public static void Enable() => PublishAfter(() => IsEnabled = true);
    public static void Disable() => PublishAfter(() => IsEnabled = false);
    public static void Toggle() => PublishAfter(() => IsEnabled = !IsEnabled);
    
    private static void PublishAfter(Action action) 
    {
        action();
        Message.Publish(new LowQualityModeChanged(IsEnabled));
    }
}
