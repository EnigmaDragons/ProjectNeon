using UnityEngine;

public class MouseHoverProcessor3D : MonoBehaviour
{
    private Camera _cam;
    private readonly RaycastHit[] _hits = new RaycastHit[100];
    private readonly MouseHoverStatusIcon _statusIcon = new MouseHoverStatusIcon();
    private Maybe<HoverCharacter> _lastHover = Maybe<HoverCharacter>.Missing();
    
    private readonly Maybe<HoverCharacter> _hoverCharacter = Maybe<HoverCharacter>.Missing();
    private readonly Maybe<Vector3> _hoverCharacterPosition = Maybe<Vector3>.Missing();
    private readonly Maybe<WorldStatusIconPresenter> _hoverStatusIcon = Maybe<WorldStatusIconPresenter>.Missing();
    
    private void Awake()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        if (UiLock.IsLocked)
            return;
        
        const int characterLayer = 10;
        const int statusIconLayer = 11;
        
        var v3 = _cam.ScreenPointToRay(Input.mousePosition);
        var numHits = Physics.RaycastNonAlloc(v3, _hits, 100f);

        _hoverCharacter.Clear();
        _hoverStatusIcon.Clear();
        _hoverCharacterPosition.Clear();
        if (numHits > 0)
        {
            for (var i = 0; i < numHits; i++)
                if (_hoverCharacter.IsMissing && _hits[i].collider.gameObject.layer == characterLayer)
                {
                    var obj = _hits[i].transform.gameObject;
                    _hoverCharacterPosition.Set(obj.transform.position, true);
                    var spriteHover = obj.GetComponentInChildren<HoverSpriteCharacter3D>();
                    var basicHover = obj.GetComponentInChildren<HoverBasicCharacter3D>();
                    if (spriteHover == null && basicHover == null)
                        Log.Error($"{obj.name} is missing a {nameof(HoverCharacter)} script");
                    else
                        _hoverCharacter.Set(spriteHover != null ? (HoverCharacter) spriteHover : basicHover, true);
                }
            
            for (var i = 0; i < numHits; i++)
                if (_hoverStatusIcon.IsMissing && _hits[i].collider.gameObject.layer == statusIconLayer)
                {
                    var obj = _hits[i].transform.gameObject;
                    var c = obj.GetComponentInChildren<WorldStatusIconPresenter>();
                    if (c == null)
                        Log.Error($"{obj.name} is missing a {nameof(WorldStatusIconPresenter)} script");
                    else
                        _hoverStatusIcon.Set(c, true);
                }
        }

        var isMouseDragging = MouseDragState.IsDragging;
        if (_lastHover == null || !_lastHover.Map(v => v.MemberId).Equals(_hoverCharacter.Map(v => v.MemberId)))
        {
            Message.Publish(new CharacterHoverChanged(_hoverCharacter.As<HoverCharacter>(), _hoverCharacterPosition, isMouseDragging));
            _lastHover = _hoverCharacter;
        }
        if (!isMouseDragging && _statusIcon != null)
            _statusIcon.Update(_hoverStatusIcon);
    }
}
