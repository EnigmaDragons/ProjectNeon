using InputIcons;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class II_CustomDeviceGraphicBehaviour : MonoBehaviour
{
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();
    private Image image => GetComponent<Image>();

    public Sprite keyboardSprite;
    public Sprite gamepadOverrideSprite;
    public Sprite nintendoSprite;
    public Sprite ps3Sprite;
    public Sprite ps4Sprite;
    public Sprite ps5Sprite;
    public Sprite xBoxSprite;

    public Sprite fallbackSprite; //if chosen sprite from above is null, use this instead

    private void OnEnable()
    {
        InputIconsManagerSO.onControlsChanged += DisplayCurrentDeviceSprite;
    }

    private void OnDisable()
    {
        InputIconsManagerSO.onControlsChanged -= DisplayCurrentDeviceSprite;
    }

    private void DisplayCurrentDeviceSprite(InputDevice inputDevice)
    {
        Sprite spriteToDisplay = GetSpriteByInputDevice(inputDevice);
        if (spriteToDisplay == null)
            spriteToDisplay = fallbackSprite;

        if (spriteRenderer)
            spriteRenderer.sprite = spriteToDisplay;

        if (image)
            image.sprite = spriteToDisplay;
    }


    public Sprite GetSpriteByInputDevice(InputDevice device)
    {
        if (!(device is Gamepad)) //device is not a gamepad, return keyboard sprite
            return keyboardSprite;

        //handle gamepads
        if (gamepadOverrideSprite != null)
            return gamepadOverrideSprite;

        if (device is UnityEngine.InputSystem.XInput.XInputController)
        {
            return xBoxSprite;
        }

#if !UNITY_STANDALONE_LINUX && !UNITY_WEBGL
        if (device is UnityEngine.InputSystem.DualShock.DualShock3GamepadHID)
        {
            return ps3Sprite;
        }

        if (device is UnityEngine.InputSystem.DualShock.DualShock4GamepadHID)
        {
            return ps4Sprite;
        }

        if (device is UnityEngine.InputSystem.DualShock.DualSenseGamepadHID) //Input System 1.2.0 or higher required (package manager dropdown menu -> see other versions)
        {
            return ps5Sprite;
        }

        if (device is UnityEngine.InputSystem.Switch.SwitchProControllerHID)
        {
            return nintendoSprite;
        }
#endif

        if (device is UnityEngine.InputSystem.DualShock.DualShockGamepad)
        {
            return ps4Sprite;
        }

        if (device.name.Contains("DualShock3"))
            return ps3Sprite;

        if (device.name.Contains("DualShock4"))
            return ps4Sprite;

        if (device.name.Contains("DualSense"))
            return ps5Sprite;

        if (device.name.Contains("ProController"))
            return nintendoSprite;

        return fallbackSprite;
    }
}

