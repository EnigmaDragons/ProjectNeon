using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class HoverCharacter : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Material _originalMaterial;
    private Member _member;

    public Member Member => _member;

    private bool _hovered;
    private Action _confirmAction;
    private Action _cancelAction;
    
    private void Update()
    {
        if (!_hovered)
            return;

        if (Input.GetMouseButtonDown(0))
            _confirmAction();
        else if (Input.GetMouseButtonDown(1)) 
            _cancelAction();
    }
    
    private void Awake()
    {
        const int characterObjectLayer = 10;
        if (gameObject.layer != characterObjectLayer)
            Log.Error($"{Member.Name} does not have the correct Game Object Layer. Must be Character.");
        if (!GetComponent<BoxCollider2D>().isTrigger)
            Log.Error($"{Member.Name}'s Box Collider must be a trigger.");
        _renderer = GetComponent<SpriteRenderer>();
        _originalMaterial = _renderer.material;
    }
    
    public void Init(Member m)
    {
        _member = m;
    }

    public void Set(Material material)
    {
        _renderer.material = material;
        _hovered = true;
    }

    public void SetAction(Action confirmAction, Action cancelAction)
    {
        _confirmAction = confirmAction;
        _cancelAction = cancelAction;
    }
    
    public void Revert()
    {
        _renderer.material = _originalMaterial;
        _hovered = false;
        _confirmAction = () => { };
        _cancelAction = () => { };
    }
}
