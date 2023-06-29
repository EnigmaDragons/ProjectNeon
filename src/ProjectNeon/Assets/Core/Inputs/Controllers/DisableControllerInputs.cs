using UnityEngine;

public class DisableControllerInputs : MonoBehaviour
{
    private void OnEnable() => Message.Publish(new DisableController());
    private void OnDisable() => Message.Publish(new EnableController());
}