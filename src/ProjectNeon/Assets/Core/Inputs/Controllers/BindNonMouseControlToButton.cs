using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BindNonMouseControlToButton : MonoBehaviour
{
    [SerializeField] private NonMouseControl[] controls;
    [SerializeField] private Button button;
    [SerializeField] private BoolVariable useController;

    private void Update()
    {
        if (!useController.Value)
            return;
        if ((controls.Contains(NonMouseControl.Confirm) && StaticControlChecker.IsConfirm()) 
                || (controls.Contains(NonMouseControl.Back) && StaticControlChecker.IsBack())
                || (controls.Contains(NonMouseControl.Change) && StaticControlChecker.IsChange())
                || (controls.Contains(NonMouseControl.Inspect) && StaticControlChecker.IsInspect())
                || (controls.Contains(NonMouseControl.Menu) && StaticControlChecker.IsMenu())
                || (controls.Contains(NonMouseControl.Next) && StaticControlChecker.IsNext())
                || (controls.Contains(NonMouseControl.Previous) && StaticControlChecker.IsPrevious())
                || (controls.Contains(NonMouseControl.Next2) && StaticControlChecker.IsNext2())
                || (controls.Contains(NonMouseControl.Previous2) && StaticControlChecker.IsPrevious2()))
            button.onClick.Invoke();
    }
}