using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(BoxCollider))]
public class HoverSpriteCharacter3D : MonoBehaviour, HoverSpriteCharacter
{
    private SpriteRenderer _renderer;
    private Material _originalMaterial;
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
            LogInfo($"UI - Confirmed {Member.NameTerm.ToEnglish()}");
            _confirmAction();
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            LogInfo($"UI - Cancelled {Member.NameTerm.ToEnglish()}");
            _cancelAction();
        }
    }
    
    public void Init(Member m)
    {
        _member = m;
        
        const int characterObjectLayer = 10;
        if (gameObject.layer != characterObjectLayer)
            Log.Error($"{Member.NameTerm.ToEnglish()} does not have the correct Game Object Layer. Must be Character.");
        if (!GetComponent<BoxCollider>().isTrigger)
            Log.Error($"{Member.NameTerm.ToEnglish()}'s Box Collider must be a trigger.");
        _renderer = GetComponent<SpriteRenderer>();
        _originalMaterial = _renderer.material;
    }

    public void Set(Material material)
    {        
        if (!IsInitialized)
            return;
        
        if (_renderer != null)
            _renderer.material = material;
        _hovered = true;
    }

    public void SetIsHovered() => _hovered = true;

    public void SetAction(Action confirmAction, Action cancelAction)
    {
        LogInfo($"UI - Set Hover Character Action for {Member.NameTerm.ToEnglish()}");
        _confirmAction = confirmAction;
        _cancelAction = cancelAction;
    }
    
    public void Revert()
    {
        if (!IsInitialized)
            return;
    
        LogInfo($"UI - Cleared Hover Character {Member.NameTerm.ToEnglish()}");
        if (_renderer != null)
            _renderer.material = _originalMaterial;
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

