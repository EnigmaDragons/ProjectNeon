using UnityEngine;
using System.Collections;

namespace UITool
{
    public class UIScreenTool
    {

        public static Rect UIToScreenRect(Canvas canvas, RectTransform rectTrans)
        {
            Vector2 pos = UIScreenTool.UIToScreenPos(canvas, rectTrans.position);
            Vector2 size = rectTrans.rect.size;// new Vector2(rectTrans.rect.width, rectTrans.rect.height);
            size = new Vector2(size.x * canvas.transform.lossyScale.x, size.y * canvas.transform.lossyScale.y);
            size = UIScreenTool.UIToScreenPos(canvas, pos + size) - UIScreenTool.UIToScreenPos(canvas, pos);

            pos = new Vector2(pos.x - rectTrans.pivot.x * size.x, pos.y - rectTrans.pivot.y * size.y);
            return new Rect(pos.x / Screen.width, pos.y / Screen.height, size.x / Screen.width, size.y / Screen.height);
        }

        public static Vector2 WorldToUIPos(Canvas canvas, Vector3 pos, Camera camera)
        {
            return ScreenToUIPos(canvas, camera.WorldToScreenPoint(pos));
        }

        public static Vector2 UIToScreenPos(Canvas canvas, Vector2 pos)
        {
            Vector2 retPos = pos;
            retPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, new Vector3(pos.x, pos.y, 0));
            return retPos;
        }

        public static Vector2 ScreenToUIPos(Canvas canvas, Vector2 pos, bool isRelative = false)
        {     
            if(isRelative)
                pos = new Vector2(pos.x * Screen.width, pos.y * Screen.height);
            Vector2 retPos = pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, retPos, canvas.worldCamera, out retPos);
            return canvas.transform.TransformPoint(retPos);
        }

        public static void ScreenToUIRect(Canvas canvas, Rect rect, RectTransform rectTransform, bool isRelative = false)
        {
            Vector2 pos1 = ScreenToUIPos(canvas, new Vector2(rect.x, rect.y), true);
            Vector2 pos2 = ScreenToUIPos(canvas, new Vector2(rect.x + rect.width, rect.y + rect.height), true);
            rectTransform.position = (pos1 + pos2) / 2;
            rectTransform.sizeDelta = (pos2 - pos1) / rectTransform.transform.lossyScale.x;
        }
    }
}
