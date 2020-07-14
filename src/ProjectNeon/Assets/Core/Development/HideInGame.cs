
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HideInGame : MonoBehaviour
{
    private void Awake() => GetComponent<Renderer>().enabled = false;
}
