using UnityEngine;
using TMPro;
 
// Resizes this object based on the preferred size of a TMP Text
[ExecuteInEditMode]
public class TextSizer : MonoBehaviour
{
    public TMP_Text text;
    public Vector2 padding;
    public Vector2 maxSize = new Vector2(1000, float.PositiveInfinity);
    public Vector2 minSize;
 
    public enum Mode
    {
        None        = 0,
        Horizontal  = 0x1,
        Vertical    = 0x2,
        Both        = Horizontal | Vertical
    }
    public Mode controlAxes = Mode.Both;
 
    protected string lastText = null;
    protected Vector2 lastSize;
    protected bool forceRefresh = false;
 
    protected virtual float MinX { get {
            if ((controlAxes & Mode.Horizontal) != 0) return minSize.x;
            return GetComponent<RectTransform>().rect.width - padding.x;
        } }
    protected virtual float MinY { get {
            if ((controlAxes & Mode.Vertical) != 0) return minSize.y;
            return GetComponent<RectTransform>().rect.height - padding.y;
        } }
    protected virtual float MaxX { get {
            if ((controlAxes & Mode.Horizontal) != 0) return maxSize.x;
            return GetComponent<RectTransform>().rect.width - padding.x;
        } }
    protected virtual float MaxY { get {
            if ((controlAxes & Mode.Vertical) != 0) return maxSize.y;
            return GetComponent<RectTransform>().rect.height - padding.y;
        } }
 
    protected virtual void Update ()
    {
        RectTransform rt = GetComponent<RectTransform>();
        if (text != null && (text.text != lastText || lastSize != rt.rect.size || forceRefresh))
        {
            lastText = text.text;
            Vector2 preferredSize = text.GetPreferredValues(MaxX, MaxY);
            preferredSize.x = Mathf.Clamp(preferredSize.x, MinX, MaxX);
            preferredSize.y = Mathf.Clamp(preferredSize.y, MinY, MaxY);
            preferredSize += padding;
 
            if ((controlAxes & Mode.Horizontal) != 0)
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, preferredSize.x);
            }
            if ((controlAxes & Mode.Vertical) != 0)
            {
                rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredSize.y);
            }
            lastSize = rt.rect.size;
            forceRefresh = false;
        }
    }
 
    // Forces a size recalculation on next Update
    public virtual void Refresh()
    {
        forceRefresh = true;
    }
}