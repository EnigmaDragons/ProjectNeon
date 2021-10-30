using UnityEngine;

public class MouseHoverProcessor3D : MonoBehaviour
{
    private Camera _cam;
    private readonly RaycastHit[] _hits = new RaycastHit[100];
    private MouseHoverStatusIcon _statusIcon;
    private Maybe<HoverCharacter> _lastHover = Maybe<HoverCharacter>.Missing();
    
    private void Awake()
    {
        _cam = Camera.main;
        _statusIcon = new MouseHoverStatusIcon();
    }

    private void Update()
    {
        if (UiLock.IsLocked)
            return;
        
        const int characterLayer = 10;
        const int statusIconLayer = 11;
        
        var v3 = _cam.ScreenPointToRay(Input.mousePosition);
        var numHits = Physics.RaycastNonAlloc(v3, _hits, 100f);

        var hoverCharacter = Maybe<HoverCharacter>.Missing();
        var hoverStatusIcon = Maybe<WorldStatusIconPresenter>.Missing();
        var hoverCharacterPosition = Maybe<Vector3>.Missing();
        if (numHits > 0)
        {
            for (var i = 0; i < numHits; i++)
                if (hoverCharacter.IsMissing && _hits[i].collider.gameObject.layer == characterLayer)
                {
                    var obj = _hits[i].transform.gameObject;
                    hoverCharacterPosition = obj.transform.position;
                    var spriteHover = obj.GetComponentInChildren<HoverSpriteCharacter3D>();
                    var basicHover = obj.GetComponentInChildren<HoverBasicCharacter3D>();
                    if (spriteHover == null && basicHover == null)
                        Log.Error($"{obj.name} is missing a {nameof(HoverCharacter)} script");
                    else
                        hoverCharacter = new Maybe<HoverCharacter>(spriteHover != null
                            ? (HoverCharacter) spriteHover
                            : basicHover);
                }
            
            for (var i = 0; i < numHits; i++)
                if (hoverStatusIcon.IsMissing && _hits[i].collider.gameObject.layer == statusIconLayer)
                {
                    var obj = _hits[i].transform.gameObject;
                    var c = obj.GetComponentInChildren<WorldStatusIconPresenter>();
                    if (c == null)
                        Log.Error($"{obj.name} is missing a {nameof(WorldStatusIconPresenter)} script");
                    else
                        hoverStatusIcon = c;
                }
        }

        var isMouseDragging = MouseDragState.IsDragging;
        if (_lastHover == null || !_lastHover.Map(v => v.MemberId).Equals(hoverCharacter.Map(v => v.MemberId)))
        {
            Message.Publish(new CharacterHoverChanged(hoverCharacter.As<HoverCharacter>(), hoverCharacterPosition, isMouseDragging));
            _lastHover = hoverCharacter;
        }
        if (!isMouseDragging && _statusIcon != null)
            _statusIcon.Update(hoverStatusIcon);
    }
}
