using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UITool
{
    public class ViewOnRect : MonoBehaviour
    {

        public Camera cam;
        private RawImage _img;
        private RectTransform _rect;
        private Vector2 _lastSize = -Vector2.one;

        // Use this for initialization
        void Start()
        {
            _img = GetComponent<RawImage>();
            _rect = GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 currSize = new Vector2(_rect.rect.width, _rect.rect.height);
            if((_lastSize - currSize).magnitude > 1)
            {
                RenderTexture tex = (RenderTexture)_img.texture;
                tex.Release();
                tex.width = (int)currSize.x;
                tex.height = (int)currSize.y;
                cam.rect = new Rect(0, 0, 1, 1);
                //cam.pixelRect = new Rect(0, 0, currSize.x, currSize.y);
                _lastSize = currSize;
            }
        }
    }
}
