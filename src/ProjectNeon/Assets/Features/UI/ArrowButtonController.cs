using UnityEngine;
using UnityEngine.UI;

public class ArrowButtonController : OnMessage<InputControlChanged>
{
    [SerializeField] private Image arrow;
    [SerializeField] private bool isLeftBumper;
    [SerializeField] private bool isRightBumper;
    [SerializeField] private bool isLeftTrigger;
    [SerializeField] private bool isRightTrigger;
    [SerializeField] private Image leftBumper;
    [SerializeField] private Image rightBumper;
    [SerializeField] private Image leftTrigger;
    [SerializeField] private Image rightTrigger;

    protected override void AfterEnable() => Update();
    protected override void Execute(InputControlChanged msg) => Update();

    private void Update()
    {
        if (InputControl.Type == ControlType.Mouse)
        {
            arrow.color = Color.white;
            leftBumper.gameObject.SetActive(false);
            rightBumper.gameObject.SetActive(false);
            leftTrigger.gameObject.SetActive(false);
            rightTrigger.gameObject.SetActive(false);
        }
        else
        {
            arrow.color = new Color(1, 1, 1, 0.5f);
            if (isLeftBumper)
                leftBumper.gameObject.SetActive(true);
            if (isRightBumper)
                rightBumper.gameObject.SetActive(true);
            if (isLeftTrigger)
                leftTrigger.gameObject.SetActive(true);
            if (isRightTrigger)
                rightTrigger.gameObject.SetActive(true);
        }
    }
}