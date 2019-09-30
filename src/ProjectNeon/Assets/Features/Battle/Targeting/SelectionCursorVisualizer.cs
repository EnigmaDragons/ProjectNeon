using UnityEngine;

public class SelectionCursorVisualizer : MonoBehaviour
{
    [SerializeField] private GameEvent activateOn;
    [SerializeField] private GameEvent deactivateOn;
    [SerializeField] private GameObject cursor;
    [SerializeField] private BattlePlayerTargetingState targeting;

    private void OnEnable()
    {
        activateOn.Subscribe(() => cursor.SetActive(true), this);
        deactivateOn.Subscribe(() => cursor.SetActive(false), this);
    }

    private void OnDisable()
    {
        activateOn.Unsubscribe(this);
        deactivateOn.Unsubscribe(this);
    }
}
