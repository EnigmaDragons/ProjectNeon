using TMPro;
using UnityEngine;

public class SwapSpriteAssetBasedOnInput : OnMessage<InputControlChanged>
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TMP_SpriteAsset keyboard;
    [SerializeField] private TMP_SpriteAsset xbox;
    [SerializeField] private TMP_SpriteAsset playstation;
    [SerializeField] private TMP_SpriteAsset switchPro;

    protected override void AfterEnable() => UpdateSpriteAsset();

    protected override void Execute(InputControlChanged msg) => UpdateSpriteAsset();

    private void UpdateSpriteAsset()
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
}