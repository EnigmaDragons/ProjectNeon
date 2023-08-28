using TMPro;
using UnityEngine;

public class SwapSpriteAssetBasedOnInput : OnMessage<InputControlChanged>
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshPro text2;
    [SerializeField] private TMP_SpriteAsset keyboard;
    [SerializeField] private TMP_SpriteAsset xbox;
    [SerializeField] private TMP_SpriteAsset playstation;
    [SerializeField] private TMP_SpriteAsset switchPro;

    protected override void AfterEnable() => UpdateSpriteAsset();

    protected override void Execute(InputControlChanged msg) => UpdateSpriteAsset();

    private void UpdateSpriteAsset()
    {
        if (text != null)
        {
            if (InputControl.Type == ControlType.Mouse || InputControl.Type == ControlType.Keyboard)
                text.spriteAsset = keyboard;
            else if (InputControl.Type == ControlType.Xbox || InputControl.Type == ControlType.Gamepad)
                text.spriteAsset = xbox;
            if (InputControl.Type == ControlType.Playstation)
                text.spriteAsset = playstation;
            if (InputControl.Type == ControlType.Switch)
                text.spriteAsset = switchPro;   
        }
        else if (text2 != null)
        {
            if (InputControl.Type == ControlType.Mouse || InputControl.Type == ControlType.Keyboard)
                text2.spriteAsset = keyboard;
            else if (InputControl.Type == ControlType.Xbox || InputControl.Type == ControlType.Gamepad)
                text2.spriteAsset = xbox;
            if (InputControl.Type == ControlType.Playstation)
                text2.spriteAsset = playstation;
            if (InputControl.Type == ControlType.Switch)
                text2.spriteAsset = switchPro;
        }
    }
}