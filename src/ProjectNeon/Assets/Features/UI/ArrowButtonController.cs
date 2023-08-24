using UnityEngine;
using UnityEngine.UI;

public class ArrowButton : OnMessage<InputControlChanged>
{
    [SerializeField] private Image arrow;
    [SerializeField] private Button button;
    [SerializeField] private bool isLeftBumper;
    [SerializeField] private bool isRightBumper;
    [SerializeField] private bool isLeftTrigger;
    [SerializeField] private bool isRightTrigger;
    [SerializeField] private Image leftBumper;
    [SerializeField] private Image rightBumper;
    [SerializeField] private Image leftTrigger;
    [SerializeField] private Image rightTrigger;
    
    
}