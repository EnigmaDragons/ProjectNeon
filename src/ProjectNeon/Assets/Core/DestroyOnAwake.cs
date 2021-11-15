
using UnityEngine;

public class DestroyOnAwake : MonoBehaviour
{
    [SerializeField] private GameObject[] targets;

    private void Awake() => targets.ForEach(Destroy);
}
