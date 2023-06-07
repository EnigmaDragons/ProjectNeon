using UnityEngine;
using UnityEngine.InputSystem;
using InputIcons;
using System;

public class II_UIRebindInputActionResetAllBindings : MonoBehaviour
{

    public InputActionAsset assetToReset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ResetBindings()
    {
        InputIconsManagerSO.ResetAllBindings();
    }
}
