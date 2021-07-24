using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class HoverBasicCharacter3D : MonoBehaviour, HoverCharacter
{
    private Member _member;

    public bool IsInitialized => _member != null;
    public Member Member => _member;
    public int MemberId => IsInitialized ? Member.Id : -1;
    
    private bool _hovered;
    private Action _confirmAction;
    private Action _cancelAction;

    private bool _loggingEnabled = false;
    
    private void Update()
    {
        if (!IsInitialized || !_hovered)
            return;
        
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            LogInfo($"UI - Confirmed {Member.Name}");
            _confirmAction();
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            LogInfo($"UI - Cancelled {Member.Name}");
            _cancelAction();
        }
    }
    
    public void Init(Member m)
    {
        _member = m;
        
        const int characterObjectLayer = 10;
        if (gameObject.layer != characterObjectLayer)
            Log.Error($"{Member.Name} does not have the correct Game Object Layer. Must be Character.");
        if (!GetComponent<BoxCollider>().isTrigger)
            Log.Error($"{Member.Name}'s Box Collider must be a trigger.");
    }
    
    public void SetIsHovered() => _hovered = true;

    public void SetAction(Action confirmAction, Action cancelAction)
    {
        LogInfo($"UI - Set Hover Character Action for {Member.Name}");
        _confirmAction = confirmAction;
        _cancelAction = cancelAction;
    }
    
    public void Revert()
    {
        if (!IsInitialized)
            return;
    
        LogInfo($"UI - Cleared Hover Character {Member.Name}");
        _hovered = false;
        _confirmAction = () => { };
        _cancelAction = () => { };
    }

    private void LogInfo(string msg)
    {
        if (_loggingEnabled)
            Log.Info(msg);
    }
}
