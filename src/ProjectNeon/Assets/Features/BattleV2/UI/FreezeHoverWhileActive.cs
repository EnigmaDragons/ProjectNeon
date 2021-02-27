using UnityEngine;

public class FreezeHoverWhileActive : MonoBehaviour
{
    private void OnEnable() => Message.Publish(new FreezeHover());
    private void OnDisable() => Message.Publish(new Finished<FreezeHover>());
}