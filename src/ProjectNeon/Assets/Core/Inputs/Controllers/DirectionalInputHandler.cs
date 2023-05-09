using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class DirectionalInputHandler : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private float holdSecondsBeforeSecondInput;
    [SerializeField] private float holdSecondsBetweenMovement;

    private List<DirectionalInputNodeMap> _maps = new List<DirectionalInputNodeMap>();
    private List<DirectionalInputNode> _selectedNodes = new List<DirectionalInputNode>();
    private GameObject _selectedGameObject;
    private float _previousVertical;
    private float _previousHorizontal;
    private InputDirection _holdingDirection = InputDirection.None;
    private float _timeTilNextMovement;
    
    private DirectionalInputNodeMap SelectedMap => _maps.Any() ? _maps[0] : null;
    private DirectionalInputNode SelectedNode => _selectedNodes.Any() ? _selectedNodes[0] : null;
    
    private void Awake()
    {
        _maps = new List<DirectionalInputNodeMap>();
        _selectedNodes = new List<DirectionalInputNode>();
        Message.Subscribe<DirectionalInputNodeMapEnabled>(Execute, this);
        Message.Subscribe<DirectionalInputNodeMapDisabled>(Execute, this);
    }

    private void Update()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        if (_selectedNodes.Any())
        {
            if ((_holdingDirection == InputDirection.Up && vertical <= 0)
                || _holdingDirection == InputDirection.Left && horizontal >= 0
                || _holdingDirection == InputDirection.Right && horizontal <= 0
                || _holdingDirection == InputDirection.Down && vertical >= 0)
                _holdingDirection = InputDirection.None;
            if (_holdingDirection != InputDirection.None)
            {
                _timeTilNextMovement -= Time.deltaTime;
                if (_timeTilNextMovement <= 0)
                {
                    _selectedNodes[0] = SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection);
                    if (SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection) == null)
                        _holdingDirection = InputDirection.None;
                    else
                        _timeTilNextMovement += holdSecondsBetweenMovement;
                }
            }
            else
            {
                if (vertical > 0 && _previousVertical <= 0 && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Up) != null)
                    _holdingDirection = InputDirection.Up;
                else if (vertical < 0 && _previousVertical >= 0 && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Down) != null)
                    _holdingDirection = InputDirection.Down;
                else if (horizontal > 0 && _previousHorizontal <= 0 && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Right) != null)
                    _holdingDirection = InputDirection.Right;
                else if (horizontal < 0 && _previousHorizontal >= 0 && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Left) != null)
                    _holdingDirection = InputDirection.Left;
                if (_holdingDirection != InputDirection.None)
                {
                    _timeTilNextMovement = holdSecondsBeforeSecondInput;
                    _selectedNodes[0] = SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection);
                    if (SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection) == null)
                        _holdingDirection = InputDirection.None;
                }
            }
            UpdateSelected();
        }
        _previousVertical = vertical;
        _previousHorizontal = horizontal;
    }
    
    private void OnDisable()
        => Message.Unsubscribe(this);

    private void Execute(DirectionalInputNodeMapEnabled msg)
    {
        var index = _maps.FirstIndexOf(x => msg.Map.Z >= x.Z);
        if (index == -1)
        {
            _maps.Add(msg.Map);
            _selectedNodes.Add(msg.Map.DefaultSelectedNode);
        }
        else
        {
            _maps.Insert(index, msg.Map);
            _selectedNodes.Insert(index, msg.Map.DefaultSelectedNode);
        }
        if (index == 0)
            _holdingDirection = InputDirection.None;
        UpdateSelected();
    }

    private void Execute(DirectionalInputNodeMapDisabled msg)
    {
        var index = _maps.IndexOf(msg.Map);
        if (index == -1)
            return;
        _maps.RemoveAt(index);
        _selectedNodes.RemoveAt(index);
        if (index == 0)
            _holdingDirection = InputDirection.None;
        UpdateSelected();
    }
    
    private void UpdateSelected()
    {
        if (_selectedGameObject != null && (_selectedNodes.Count == 0 || _selectedNodes[0].Selectable != _selectedGameObject))
        {
            _selectedGameObject = null;
            eventSystem.SetSelectedGameObject(_selectedGameObject);
        }
        if (_selectedGameObject == null && _selectedNodes.Count > 0)
        {
            _selectedGameObject = _selectedNodes[0].Selectable;
            eventSystem.SetSelectedGameObject(_selectedGameObject);
        }
    }
}