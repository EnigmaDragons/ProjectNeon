using UnityEngine;

public class SendErrorReportForUnhandledExceptions : MonoBehaviour
{
    [SerializeField] private bool performTest = false;
    
    private void Awake() => ErrorHandler.SetErrorAction(ErrorReport.Send);

    private void Update()
    {
        if (performTest)
        {
            performTest = false;
            this.ExecuteAfterDelay(ErrorReport.Test, 1f);
        }
    }
}
