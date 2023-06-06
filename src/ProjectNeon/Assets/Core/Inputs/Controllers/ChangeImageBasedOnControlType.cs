using UnityEngine;
using UnityEngine.UI;

public class ChangeImageBasedOnControlType : OnMessage<InputControlChanged>
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite mouseSprite;
    [SerializeField] private Sprite keyboardSprite;
    [SerializeField] private Sprite xboxSprite;

    private void Awake() => UpdateSprite();

    protected override void Execute(InputControlChanged msg) => UpdateSprite();

    private void UpdateSprite()
    {
        if (InputControl.Type == ControlType.Mouse)
            image.sprite = mouseSprite;
        else if (InputControl.Type == ControlType.Keyboard)
            image.sprite = keyboardSprite;
        else if (InputControl.Type == ControlType.Xbox)
            image.sprite = xboxSprite;
    }
}