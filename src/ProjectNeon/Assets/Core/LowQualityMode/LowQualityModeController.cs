
public class LowQualityModeController : OnMessage<SetLowQualityMode>
{
    protected override void Execute(SetLowQualityMode msg)
    {
        if (msg.Operation == BooleanControlRequest.Enable)
            CurrentLowQualityMode.Enable();
        if (msg.Operation == BooleanControlRequest.Disable)
            CurrentLowQualityMode.Disable();
        if (msg.Operation == BooleanControlRequest.Toggle)
            CurrentLowQualityMode.Toggle();
    }
}