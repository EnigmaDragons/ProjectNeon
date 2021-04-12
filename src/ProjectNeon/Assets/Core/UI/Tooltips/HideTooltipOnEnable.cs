using UnityEngine;

public class HideTooltipOnEnable : MonoBehaviour
{
    private void OnEnable() => Message.Publish(new HideTooltip());
}
