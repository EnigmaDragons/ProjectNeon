using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindNonMouseControlToButton : MonoBehaviour
{
    [SerializeField] private NonMouseControl[] controls;
    [SerializeField] private Button button;
    [SerializeField] private BoolVariable useController;

    private HashSet<NonMouseControl> _controlSet;

    private void Awake() => _controlSet = new HashSet<NonMouseControl>(controls);
        
    private void Update()
    {
        if (!useController.Value)
            return;
        if ((_controlSet.Contains(NonMouseControl.Confirm) && StaticControlChecker.IsConfirm()) 
                || (_controlSet.Contains(NonMouseControl.Back) && StaticControlChecker.IsBack())
                || (_controlSet.Contains(NonMouseControl.Change) && StaticControlChecker.IsChange())
                || (_controlSet.Contains(NonMouseControl.Inspect) && StaticControlChecker.IsInspect())
                || (_controlSet.Contains(NonMouseControl.Menu) && StaticControlChecker.IsMenu())
                || (_controlSet.Contains(NonMouseControl.Next) && StaticControlChecker.IsNext())
                || (_controlSet.Contains(NonMouseControl.Previous) && StaticControlChecker.IsPrevious())
                || (_controlSet.Contains(NonMouseControl.Next2) && StaticControlChecker.IsNext2())
                || (_controlSet.Contains(NonMouseControl.Previous2) && StaticControlChecker.IsPrevious2()))
            button.onClick.Invoke();
    }
}