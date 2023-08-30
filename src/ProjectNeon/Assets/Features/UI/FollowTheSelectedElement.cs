using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FollowTheSelectedElement : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    
    private EventSystem _eventSystem;

    private void Awake() => _eventSystem = EventSystem.current;
    
    private void Update()
    {
        if (InputControl.Type == ControlType.Mouse || _eventSystem.currentSelectedGameObject == null || !scroll.gameObject.activeInHierarchy)
            return;
        var selectedObject = _eventSystem.currentSelectedGameObject;
        var selectedTransform = selectedObject.transform;
        var y = (int) Math.Round(selectedTransform.localPosition.y);
        var hashset = new HashSet<int>();
        foreach (Transform child in selectedTransform.parent)
            hashset.Add((int) Math.Round(child.localPosition.y));
        var orderedHashset = hashset.OrderBy(x => x).ToArray();
        var index = orderedHashset.FirstIndexOf(x => y == x);
        scroll.verticalNormalizedPosition = orderedHashset.Length <= 1 ? 0 : index / (orderedHashset.Length - 1f);
    }
}