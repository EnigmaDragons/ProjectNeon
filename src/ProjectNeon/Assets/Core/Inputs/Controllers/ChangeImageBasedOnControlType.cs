using UnityEngine;
using UnityEngine.UI;

public class ChangeImageBasedOnControlType : OnMessage<InputControlChanged>
{
    [SerializeField] private Image image;
    [SerializeField] private ControlTypeSprite sprite;
    [SerializeField] private BoolReference featureFlag;
    
    protected override void AfterEnable() => UpdateSprite();

    protected override void Execute(InputControlChanged msg) => UpdateSprite();

    private void UpdateSprite()
    {
        if (!featureFlag.Value)
            return;
        image.sprite = sprite.Get();
    }
}