using System.Linq;
using UnityEngine;

public class MouseHoverCharacterTarget : MonoBehaviour
{
    [SerializeField] private Material hoverMaterial;
    
    private Camera _cam;
    private Maybe<HoverCharacter> _char = Maybe<HoverCharacter>.Missing();
    private readonly RaycastHit2D[] _hits = new RaycastHit2D[100];

    private void Awake() => _cam = Camera.main;

    private void Update()
    {
        const int characterLayer = 10;
        var wp = _cam.ScreenToWorldPoint(Input.mousePosition);
        var v2 = new Vector2(wp.x, wp.y);
        if (Physics2D.RaycastNonAlloc(v2, Vector2.zero, _hits, 100f) > 0)
        {
            var firstHit = _hits.FirstOrDefault(x => (x.collider?.gameObject?.layer ?? 0) == characterLayer);
            var hoverChar = firstHit.transform.gameObject.GetComponentInChildren<HoverCharacter>();
            if (hoverChar == null)
            {
                Log.Error($"{firstHit.transform.gameObject.name} is missing a {nameof(HoverCharacter)} script");
            }
            else
            {
                if (_char.IsPresent && _char.Value != hoverChar)
                    ResetLast();
                _char = hoverChar;
                _char.Value.Set(hoverMaterial);
            }
        }
        else
        {
            ResetLast();
        }
    }

    private void ResetLast()
    {
        if (_char.IsPresent)
        {
            _char.Value.Revert();
            _char = Maybe<HoverCharacter>.Missing();
        }
    }
}
