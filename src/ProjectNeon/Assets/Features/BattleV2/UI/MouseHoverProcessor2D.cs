using UnityEngine;

public class MouseHoverProcessor2D : MonoBehaviour
{
    private Camera _cam;
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[100];
    private MouseHoverStatusIcon _statusIcon;
    private Maybe<HoverCharacter2D> _lastHover = Maybe<HoverCharacter2D>.Missing();
    
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

        var hoverCharacter = Maybe<HoverCharacter2D>.Missing();
        var hoverStatusIcon = Maybe<WorldStatusIconPresenter>.Missing();
        if (numHits > 0)
        {
            for (var i = 0; i < numHits; i++)
                if (hoverCharacter.IsMissing && _hits[i].collider.gameObject.layer == characterLayer)
                {
                    var obj = _hits[i].transform.gameObject;
                    var c = obj.GetComponentInChildren<HoverCharacter2D>();
                    if (c == null)
                        Log.Error($"{obj.name} is missing a {nameof(HoverCharacter2D)} script");
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

        if ((_lastHover.IsMissing && hoverCharacter.IsPresent)
            || (_lastHover.IsPresent && hoverCharacter.IsMissing)
            || (_lastHover.IsPresent && hoverCharacter.IsPresent &&
                _lastHover.Value.Member.Id != hoverCharacter.Value.Member.Id))
        {
            Message.Publish(new CharacterHoverChanged { HoverCharacter = hoverCharacter.As<HoverCharacter>() });
            _lastHover = hoverCharacter;
        }
        _statusIcon.Update(hoverStatusIcon);
    }
}
