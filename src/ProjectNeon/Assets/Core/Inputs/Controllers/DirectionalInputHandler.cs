using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DirectionalInputHandler : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private float holdSecondsBeforeSecondInput;
    [SerializeField] private float holdSecondsBetweenMovement;
    [SerializeField] private FloatReference minimumMovementBeforeDirectionCounted;
    [SerializeField] private BoolReference featureFlag;

    private List<DirectionalInputNodeMap> _maps = new List<DirectionalInputNodeMap>();
    private List<DirectionalInputNode> _selectedNodes = new List<DirectionalInputNode>();
    private GameObject _selectedGameObject;
    private InputDirection _previousDirection = InputDirection.None;
    private InputDirection _holdingDirection = InputDirection.None;
    private float _timeTilNextMovement;
    private bool _disabled;
    
    private DirectionalInputNodeMap SelectedMap => _maps.Any() ? _maps[0] : null;
    private DirectionalInputNode SelectedNode => _selectedNodes.Any() ? _selectedNodes[0] : null;
    
    private void Awake()
    {
        _disabled = !featureFlag.Value;
        _maps = new List<DirectionalInputNodeMap>();
        _selectedNodes = new List<DirectionalInputNode>();
        Message.Subscribe<DirectionalInputNodeMapEnabled>(Execute, this);
        Message.Subscribe<DirectionalInputNodeMapChanged>(Execute, this);
        Message.Subscribe<DirectionalInputNodeMapDisabled>(Execute, this);
        Message.Subscribe<DisableController>(Execute, this);
        Message.Subscribe<EnableController>(Execute, this);
        Message.Subscribe<InputControlChanged>(_ => UpdateSelected(), this);
    }

    private void Update()
    {
        if (!_maps.Any() || _disabled || InputControl.Type == ControlType.Mouse)
            return;
        
        var changedSelection = false;
        var vertical = GetVertical();
        var horizontal = GetHorizontal();
        var direction = InputDirection.None;
        if (Math.Abs(vertical) >= Math.Abs(horizontal) && vertical > minimumMovementBeforeDirectionCounted.Value)
            direction = InputDirection.Up;
        else if (Math.Abs(vertical) >= Math.Abs(horizontal) && vertical < -minimumMovementBeforeDirectionCounted.Value)
            direction = InputDirection.Down;
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal > minimumMovementBeforeDirectionCounted.Value)
            direction = InputDirection.Right;
        else if (Math.Abs(vertical) < Math.Abs(horizontal) && horizontal < -minimumMovementBeforeDirectionCounted.Value)
            direction = InputDirection.Left;

        if (_selectedNodes.Any())
        {
            if ((_holdingDirection == InputDirection.Up && direction != InputDirection.Up)
                || (_holdingDirection == InputDirection.Left && direction != InputDirection.Left)
                || (_holdingDirection == InputDirection.Right && direction != InputDirection.Right)
                || (_holdingDirection == InputDirection.Down && direction != InputDirection.Down))
                _holdingDirection = InputDirection.None;
            if (_holdingDirection != InputDirection.None)
            {
                _timeTilNextMovement -= Time.deltaTime;
                if (_timeTilNextMovement <= 0)
                {
                    _selectedNodes[0] = SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection);
                    changedSelection = true;
                    if (SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection) == null)
                        _holdingDirection = InputDirection.None;
                    else
                        _timeTilNextMovement += holdSecondsBetweenMovement;
                }
            }
            else
            {
                if (direction == InputDirection.Up && _previousDirection != InputDirection.Up && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Up) != null)
                    _holdingDirection = InputDirection.Up;
                else if (direction == InputDirection.Down && _previousDirection != InputDirection.Down && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Down) != null)
                    _holdingDirection = InputDirection.Down;
                else if (direction == InputDirection.Right && _previousDirection != InputDirection.Right && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Right) != null)
                    _holdingDirection = InputDirection.Right;
                else if (direction == InputDirection.Left && _previousDirection != InputDirection.Left && SelectedMap.GetNodeInDirection(SelectedNode, InputDirection.Left) != null)
                    _holdingDirection = InputDirection.Left;
                if (_holdingDirection != InputDirection.None)
                {
                    _timeTilNextMovement = holdSecondsBeforeSecondInput;
                    _selectedNodes[0] = SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection);
                    changedSelection = true;
                    if (SelectedMap.GetNodeInDirection(SelectedNode, _holdingDirection) == null)
                        _holdingDirection = InputDirection.None;
                }
            }
            UpdateSelected();
        }
        _previousDirection = direction;
        
        if (StaticControlChecker.IsConfirm())
            ActivateControl(_selectedGameObject);
        else if (StaticControlChecker.IsBack())
            ActivateControl(_maps[0].BackObject);
        else if (StaticControlChecker.IsNext())
            ActivateControl(_maps[0].NextObject);
        else if (StaticControlChecker.IsPrevious())
            ActivateControl(_maps[0].PreviousObject);
        else if (StaticControlChecker.IsNext2())
            ActivateControl(_maps[0].NextObject2);
        else if (StaticControlChecker.IsPrevious2())
            ActivateControl(_maps[0].PreviousObject2);
        else if (StaticControlChecker.IsChange())
            ActivateAlternateControl(_selectedGameObject);
        else if (StaticControlChecker.IsInspect())
        {
            if (_maps[0].InspectObject != null)
                ActivateControl(_maps[0].InspectObject);
            else
                ActivateInspectControl(_selectedGameObject);

        }
        else if (!changedSelection && _selectedGameObject != null && _selectedGameObject.TryGetComponent<Slider>(out var slider))
        {
            if (horizontal > minimumMovementBeforeDirectionCounted)
                slider.value += 0.01f;
            else if (horizontal < -minimumMovementBeforeDirectionCounted)
                slider.value -= 0.01f;
        }
    }

    private float GetVertical()
    {
        var vertical = Input.GetAxisRaw("Vertical");
        if (InputControl.Type == ControlType.Xbox && Math.Abs(vertical) < minimumMovementBeforeDirectionCounted.Value)
            vertical = Input.GetAxisRaw("Axis7");
        if (InputControl.Type == ControlType.Gamepad && Math.Abs(vertical) < minimumMovementBeforeDirectionCounted.Value)
            vertical = -Input.GetAxisRaw("Axis8");
        return vertical;
    }

    private float GetHorizontal()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        if (InputControl.Type == ControlType.Xbox && Math.Abs(horizontal) < minimumMovementBeforeDirectionCounted.Value)
            horizontal = Input.GetAxisRaw("Axis6");
        if (InputControl.Type == ControlType.Gamepad && Math.Abs(horizontal) < minimumMovementBeforeDirectionCounted.Value)
            horizontal = Input.GetAxisRaw("Axis7");
        return horizontal;
    }

    private void ActivateControl(GameObject control)
    {
        if (control == null || !control.activeInHierarchy)
            return;
        if (control.TryGetComponent<ConfirmActionComponent>(out var action))
            action.Execute();
        else if (control.TryGetComponent<Button>(out var button))
        {
            if (button.enabled)
                button.onClick.Invoke();   
        }
        else if (control.TryGetComponent<Toggle>(out var toggle))
            toggle.isOn = !toggle.isOn;
        else if (control.TryGetComponent<OnClickAction>(out var clickAction))
            clickAction.Click();
        else if (control.TryGetComponent<TMP_Dropdown>(out var dropdown))
        {
            if (dropdown.IsExpanded)
                dropdown.Hide();
            else
                dropdown.Show();
        }
    }

    private void ActivateAlternateControl(GameObject control)
    {
        if (control == null)
            return;
        if (control.TryGetComponent<AlternateActionComponent>(out var action))
            action.Execute();
    }

    private void ActivateInspectControl(GameObject control)
    {
        if (control == null)
            return;
        if (control.TryGetComponent<InspectActionComponent>(out var action))
            action.Execute();
    }

    private void OnDisable()
        => Message.Unsubscribe(this);

    private void Execute(DirectionalInputNodeMapEnabled msg)
    {
        if (msg.Map.DefaultSelectedNode == null)
            return;
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
        if (index == 0 || _maps.Count == 1)
            _holdingDirection = InputDirection.None;
        UpdateSelected();
    }

    private void Execute(DirectionalInputNodeMapChanged msg)
    {
        if (msg.UpdatedMap.DefaultSelectedNode == null)
        {
            Execute(new DirectionalInputNodeMapDisabled(msg.OutdatedMap));
            return;
        }
        var index = _maps.IndexOf(msg.OutdatedMap);
        if (index == -1)
        {
            Execute(new DirectionalInputNodeMapEnabled(msg.UpdatedMap));
            return;
        }
        _maps[index] = msg.UpdatedMap;
        _selectedNodes[index] = _selectedNodes[index].Selectable != null && _selectedNodes[index].Selectable.activeInHierarchy && msg.UpdatedMap.Nodes.Any(x => x.Selectable == _selectedNodes[index].Selectable)
            ? msg.UpdatedMap.Nodes.First(x => x.Selectable == _selectedNodes[index].Selectable)
            : msg.UpdatedMap.DefaultSelectedNode;
        if (index == 0 || _maps.Count == 1)
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

    private void Execute(DisableController msg)
    {
        _disabled = true;
        eventSystem.SetSelectedGameObject(null);
    }
    
    private void Execute(EnableController msg)
    {
        _disabled = !featureFlag.Value;
        eventSystem.SetSelectedGameObject(_selectedGameObject);
        UpdateSelected();
    }
    
    private void UpdateSelected()
    {
        if (_disabled)
            return;
        if (InputControl.Type == ControlType.Mouse)
        {
            if (_selectedGameObject != null)
            {
                _selectedGameObject = null;
                eventSystem.SetSelectedGameObject(_selectedGameObject);   
            }
            return;
        }
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