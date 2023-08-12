using UnityEngine;
using UnityEngine.UI;

public class BindNonMouseControlToButton : MonoBehaviour
{
    [SerializeField] private NonMouseControl control;
    [SerializeField] private Button button;

    private void Update()
    {
        if ((control == NonMouseControl.Confirm && StaticControlChecker.IsConfirm()) 
                || (control == NonMouseControl.Back && StaticControlChecker.IsBack())
                || (control == NonMouseControl.Change && StaticControlChecker.IsChange())
                || (control == NonMouseControl.Inspect && StaticControlChecker.IsInspect())
                || (control == NonMouseControl.Menu && StaticControlChecker.IsMenu()))
            button.onClick.Invoke();
    }
}