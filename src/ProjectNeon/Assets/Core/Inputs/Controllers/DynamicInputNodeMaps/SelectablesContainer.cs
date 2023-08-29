using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SelectablesContainer : MonoBehaviour
{
    [SerializeField] private int targetDepth = 1;
    
    private Action _onChange;
    private List<Vector3> _positions;
    private List<RectTransform> _transforms;
    private RectTransform[] _results = Array.Empty<RectTransform>();

    public void Observe(Action onChange) => _onChange = onChange;

    public RectTransform[] GetSelectables() => _results ?? Array.Empty<RectTransform>();

    public GameObject GetDefault()
    {
        try
        {
            return (_results == null || _results.Length == 0 || _results[0] == null)
                ? null
                : _results[0].gameObject;
        }
        catch (Exception e)
        {
            Log.NonCrashingError(e);
            return null;
        }
    }

    private void Update()
    {
        var positions = new List<Vector3>();
        var transforms = new List<RectTransform>();
        var i = 0;
        var changed = false;
        var children = GetChildren(transform, targetDepth);
        foreach (var child in children)
        {
            if (_transforms == null || _transforms.Count <= i || child != _transforms[i] || Vector3.Distance(child.position, _positions[i]) > 1)
                changed = true;    
            positions.Add(child.position);
            transforms.Add(child);
            i++;
        }
        if (changed)
        {
            _transforms = transforms;
            _positions = positions;
            _results = _transforms.Select(x =>
            {
                var selectable = x.GetComponentInChildren<SelectableComponent>();
                return selectable == null ? null : selectable.GetComponent<RectTransform>();
            }).Where(x => x != null).ToArray();
            if (_onChange != null)
                _onChange();
        }
    }

    private List<RectTransform> GetChildren(Transform trans, int depth)
    {
        var transforms = new List<RectTransform>();
        foreach (RectTransform child in trans)
        {
            if (child.transform == transform || !child.gameObject.activeInHierarchy)
                continue;
            if (depth == 1)
                transforms.Add(child);
            else
                transforms.AddRange(GetChildren(child, depth - 1));
        }
        return transforms;
    }
}