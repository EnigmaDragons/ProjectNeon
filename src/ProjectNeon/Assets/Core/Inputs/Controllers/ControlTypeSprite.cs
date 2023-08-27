using UnityEngine;

[CreateAssetMenu(menuName = "Control Type Image")]
public class ControlTypeSprite : ScriptableObject
{
    [SerializeField] private Sprite mouseSprite;
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite xboxSprite;
    [SerializeField] private Sprite playstationSprite;
    [SerializeField] private Sprite switchSprite;

    public Sprite Get()
    {
        if (InputControl.Type == ControlType.Mouse)
            return mouseSprite;
        if (InputControl.Type == ControlType.Keyboard)
            return keyboardSprite;
        if (InputControl.Type == ControlType.Xbox)
            return xboxSprite;
        if (InputControl.Type == ControlType.Gamepad)
            return xboxSprite;
        if (InputControl.Type == ControlType.Playstation)
            return playstationSprite;
        if (InputControl.Type == ControlType.Switch)
            return switchSprite;
        return mouseSprite;
    }
}