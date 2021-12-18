using UnityEngine;

public class InfiniteScrollingBuildings : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject initial;
    [SerializeField] private Vector3 repeatDimension;
    [SerializeField] private Vector3 spawnPos;
    [SerializeField] private Vector3 repeatTriggerPosition;

    private GameObject _main;
    
    private void Start()
    {
        _main = initial;
        if (_main == null)
            SpawnNew();
    }

    private void SpawnNew()
    {
        var obj = Instantiate(prefab, spawnPos, Quaternion.identity, parent.transform);
        obj.GetComponentInChildren<OnPositionReached>().SetAction(() => Destroy(obj));
        _main = obj;
    }

    private void FixedUpdate()
    {
        if (_main == null)
            return;
        
        var pos = _main.transform.position;
        var dimensions = repeatDimension;
        var boundaries = repeatTriggerPosition;
        if (dimensions.x < 0 && pos.x <= boundaries.x)
            Trigger();
        if (dimensions.x > 0 && pos.x >= boundaries.x)
            Trigger();
        if (dimensions.y < 0 && pos.y <= boundaries.y)
            Trigger();
        if (dimensions.y > 0 && pos.y >= boundaries.y)
            Trigger();
        if (dimensions.z < 0 && pos.z <= boundaries.z)
            Trigger();
        if (dimensions.z > 0 && pos.z >= boundaries.z)
            Trigger();
    }

    private void Trigger() => SpawnNew();
}
