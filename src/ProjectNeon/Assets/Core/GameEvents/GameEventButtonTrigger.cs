using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public sealed class GameEventButtonTrigger : MonoBehaviour
{
    [SerializeField] private GameEvent onClick;

    private void Awake() => GetComponent<Button>().onClick.AddListener(() => onClick.Publish());
}
