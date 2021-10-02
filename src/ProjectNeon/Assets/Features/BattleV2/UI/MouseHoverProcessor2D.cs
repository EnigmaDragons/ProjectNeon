using UnityEngine;

public class MouseHoverProcessor2D : MonoBehaviour
{
    private Camera _cam;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[100];
    private MouseHoverStatusIcon _statusIcon;
    private Maybe<HoverSpriteCharacter2D> _lastHover = Maybe<HoverSpriteCharacter2D>.Missing();
    
    private void Awake()
    {
        _cam = Camera.main;
        _statusIcon = new MouseHoverStatusIcon();
    }

    private void Update()
    {
        const int characterLayer = 10;
        const int statusIconLayer = 11;
        
        var wp = _cam.ScreenToWorldPoint(Input.mousePosition);
        var v2 = new Vector2(wp.x, wp.y);
        var numHits = Physics2D.RaycastNonAlloc(v2, Vector2.zero, _hits, 100f);

        var hoverCharacter = Maybe<HoverSpriteCharacter2D>.Missing();
        var hoverStatusIcon = Maybe<WorldStatusIconPresenter>.Missing();
        var hoverCharacterPosition = Maybe<Vector3>.Missing();
        if (numHits > 0)
        {
            for (var i = 0; i < numHits; i++)
                if (hoverCharacter.IsMissing && _hits[i].collider.gameObject.layer == characterLayer)
                {
                    var obj = _hits[i].transform.gameObject;
                    hoverCharacterPosition = obj.transform.position;
                    var c = obj.GetComponentInChildren<HoverSpriteCharacter2D>();
                    if (c == null)
                        Log.Error($"{obj.name} is missing a {nameof(HoverSpriteCharacter2D)} script");
                    else
                        hoverCharacter = c;
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
        if (_lastHover == null || !_lastHover.Map(v => v.Member.Id).Equals(hoverCharacter.Map(v => v.Member.Id)))
        {
            Message.Publish(new CharacterHoverChanged(hoverCharacter.As<HoverCharacter>(), hoverCharacterPosition, isMouseDragging));
            _lastHover = hoverCharacter;
        }
        if (!isMouseDragging && _statusIcon != null)
            _statusIcon.Update(hoverStatusIcon);
    }
}
