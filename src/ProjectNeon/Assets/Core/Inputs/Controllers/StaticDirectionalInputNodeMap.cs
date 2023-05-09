using UnityEngine;

public class StaticDirectionalInputNodeMap : MonoBehaviour
{
    [SerializeField] public DirectionalInputNodeMap nodeMap;

    private void OnEnable()
        => Message.Publish(new DirectionalInputNodeMapEnabled(nodeMap));

    private void OnDisable()
        => Message.Publish(new DirectionalInputNodeMapDisabled(nodeMap));
}