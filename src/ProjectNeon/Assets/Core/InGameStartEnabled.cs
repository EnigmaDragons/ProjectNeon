using UnityEngine;

public sealed class InGameStartEnabled : MonoBehaviour
{
    [SerializeField] private GameObject target;

    void Awake() => target.SetActive(true);
}
