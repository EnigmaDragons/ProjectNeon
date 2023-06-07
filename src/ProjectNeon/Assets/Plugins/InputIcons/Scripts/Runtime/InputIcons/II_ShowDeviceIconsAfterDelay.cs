using InputIcons;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class II_ShowDeviceIconsAfterDelay : MonoBehaviour
{
    private Coroutine deviceChangeCoroutine;
    private bool deviceChangeScheduled = false;


    public void ScheduleDeviceDisplayChange(InputDevice newDevice, float delay)
    {
        if (deviceChangeScheduled)
            return;

        deviceChangeCoroutine = StartCoroutine(WaitAndShowDevice(newDevice, delay));
        deviceChangeScheduled = true;
    }

    public void CancelScheduledDeviceChange()
    {
        deviceChangeScheduled = false;
        if (deviceChangeCoroutine != null)
        {
            StopCoroutine(deviceChangeCoroutine);
        }
    }

    IEnumerator WaitAndShowDevice(InputDevice device, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        InputIconsManagerSO.SetDeviceAndRefreshDisplayedIcons(device);
        deviceChangeScheduled = false;
    }
  
    public void ShowGamepadIconsAfterDelay()
    {
        StartCoroutine(WaitAndSwitchToGamepadIcons());
    }

    IEnumerator WaitAndSwitchToGamepadIcons()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        InputIconsManagerSO.ShowGamepadIconsIfGamepadAvailable();
    }

    public void ShowKeyboardIconsAfterDelay()
    {
        StartCoroutine(WaitAndSwitchToKeyboardIcons());
    }

    IEnumerator WaitAndSwitchToKeyboardIcons()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        InputIconsManagerSO.ShowKeyboardIconsIfKeyboardAvailable();
    }

    public void DestroyAfterDelay(float delay)
    {
        Destroy(this.gameObject, delay);
    }
}
