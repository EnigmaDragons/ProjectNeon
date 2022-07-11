using UnityEngine;

public class HideNamedElementsWhileActive : MonoBehaviour
{
    [SerializeField] private StringReference[] targets;

    private void OnEnable() => targets.ForEach(t => Message.Publish(new HideNamedTarget(t)));

    private void OnDisable() => targets.ForEach(t => Message.Publish(new ShowNamedTarget(t)));
}
