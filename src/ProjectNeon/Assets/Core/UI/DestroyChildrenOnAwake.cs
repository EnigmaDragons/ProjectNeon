using UnityEngine;

public class DestroyChildrenOnAwake : MonoBehaviour
{
    private void Awake() => gameObject.DestroyAllChildren();
}