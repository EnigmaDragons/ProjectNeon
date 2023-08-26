using UnityEngine;

public class DisableIfMouseControls : OnMessage<InputControlChanged>
{
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShow;

    protected override void Execute(InputControlChanged msg) => UpdateActive();
    protected override void AfterEnable() => UpdateActive();
    
    private void UpdateActive()
    {
        objectsToHide.ForEach(x => x.gameObject.SetActive(InputControl.Type != ControlType.Mouse));
        objectsToShow.ForEach(x => x.gameObject.SetActive(InputControl.Type == ControlType.Mouse));
    }
}