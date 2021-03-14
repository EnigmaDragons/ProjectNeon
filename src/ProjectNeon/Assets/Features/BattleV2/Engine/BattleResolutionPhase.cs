using System;
using System.Collections;
using UnityEngine;

[Obsolete]
public class BattleResolutionPhase : MonoBehaviour
{
    [SerializeField] private BattleUiVisuals ui;
    [SerializeField] private BattleResolutions resolutions;
    [SerializeField] private FloatReference delay = new FloatReference(1.5f);
    
    public IEnumerator Begin()
    {
        yield break;
//        DevLog.Write($"Card Resolution Began");
//        yield return ui.BeginResolutionPhase();
//        yield return new WaitForSeconds(delay);
    }
}
