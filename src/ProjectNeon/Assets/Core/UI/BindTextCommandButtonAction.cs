
using UnityEngine;
using UnityEngine.Events;

public class BindTextCommandButtonAction : MonoBehaviour
{
    [SerializeField] private TextCommandButton btn;
    [SerializeField] private string text;
    [SerializeField] private UnityEvent action;

    private void Awake() => btn.Init(text, () => action.Invoke());
}
