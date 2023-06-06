using UnityEngine;
using UnityEngine.UI;

public class ChangeImageIfController : OnMessage<InputControlChanged>
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite nonControllerSprite;
    [SerializeField] private Sprite controllerSprite;
    
    protected override void Execute(InputControlChanged msg)
    {
    }
}