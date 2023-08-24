using UnityEngine;

public class StaticDirectionalInputNodeMap : MonoBehaviour
{
    [SerializeField] public DirectionalInputNodeMap nodeMap;

    private bool _isActive;

    private void Awake() => nodeMap.Nodes.ForEach(x =>
    {
        var observable = x.Selectable.GetComponent<EnableStateObservable>();
        if (observable != null)
            observable.Watch(() =>
            {
                if (_isActive)
                    Message.Publish(new DirectionalInputNodeMapChanged(nodeMap, nodeMap));
            });
    });
    
    private void OnEnable()
    {
        if (nodeMap.DefaultSelectedNode == null)
            return;
        _isActive = true;
        Message.Publish(new DirectionalInputNodeMapEnabled(nodeMap));
    }

    private void Update()
    {
        if (_isActive)
            return;
        OnEnable();
    }

    private void OnDisable()
    {
        _isActive = false;
        Message.Publish(new DirectionalInputNodeMapDisabled(nodeMap));
    }
}