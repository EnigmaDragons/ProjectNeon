using System;
using UnityEngine;

public abstract class ProgressiveText : MonoBehaviour
{
    public abstract void Hide();
    public abstract void ForceHide();
    public abstract void Display(string text, bool shouldAutoProceed, bool manualInterventionDisablesAuto, Action onFinished);
    public abstract void Proceed(bool isAuto);
    public abstract void SetAllowManualAdvance(bool allow);
    public abstract void SetDisplayReversed(bool reversed);
    public abstract void SetOnFullyShown(Action action);

    public void Display(string text) 
        => Display(text,  false, true, () => { });
    public void Display(string text, Action onFinished) 
        => Display(text, false, true, onFinished);
    public void Display(string text, bool shouldAutoProceed, Action onFinished) 
        => Display(text, shouldAutoProceed, true, onFinished);

    public void Proceed() => Proceed(false);
}
