using UnityEngine;

public class FreezeMapWhileActive : MonoBehaviour
{
    private void OnEnable() => Message.Publish(new FreezeMap());
    private void OnDisable() => Message.Publish(new Finished<FreezeMap>());
}