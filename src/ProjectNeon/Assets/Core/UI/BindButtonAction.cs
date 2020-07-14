using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class BindButtonAction : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private UnityEvent action;

    void Awake() => button.onClick.AddListener(() => action.Invoke());
}
