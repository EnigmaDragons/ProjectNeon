using System;
using I2.Loc;
using UnityEngine;

public class I2ProgressiveTextRevealUi : ProgressiveTextRevealUi
{
    [SerializeField] private Localize localize;
    
    public override void Display(string text, bool shouldAutoProceed, bool manualInterventionDisablesAuto, Action onFinished) 
    {
        localize.SetTerm("");
        base.Display(text, shouldAutoProceed, manualInterventionDisablesAuto, onFinished);
    }
}