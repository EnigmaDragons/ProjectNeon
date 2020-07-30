using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace UITool
{
    [ExecuteInEditMode]
    public class WindowLogic : MonoBehaviour
    {
        protected RectTransform currRect;
        private RectTransform parentRect;
        //TODO Calculate scale
        //private float scale;

        [SerializeField]
        protected Canvas canvas;
        [SerializeField]
        private Button header;
        [SerializeField]
        private Button[] sizers;
        [SerializeField]
        private RectTransform content;
        private int _sizeBorders = 0;
        public int SizeBorders = 10;
        private List<EventTrigger> triggers = new List<EventTrigger>();
        public bool isBodyMove = false;
        public bool isRotate;

        public Vector2 minSize = Vector2.zero;
        public Vector2 maxSize = Vector2.zero;
        private Vector2 currSize;

        // Use this for initialization
        public virtual void Start()
        {
            currRect = GetComponent<RectTransform>();
            parentRect = transform.parent.gameObject.GetComponent<RectTransform>();
            /*EventTrigger[] OldTriggers = header.gameObject.GetComponents<EventTrigger>();
            for (int j = 1; j < OldTriggers.Length; j++)
            {
                Destroy(OldTriggers[j]);
            }*/
            EventTrigger trigger = header.gameObject.GetComponent<EventTrigger>();
            var pointerDown = trigger.triggers[0];
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((e) => OnMouseDown());

            var pointerUp = trigger.triggers[1];
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((e) => OnMouseUp());

            var pointerEnter = trigger.triggers[2];
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((e) => OnMouseOver());

            var pointerExit = trigger.triggers[3];
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((e) => OnMouseExit());

            if (isBodyMove)
            {
                EventTrigger triggerContent = content.gameObject.GetComponent<EventTrigger>();
                var pointerDownContent = triggerContent.triggers[0];// new EventTrigger.Entry();
                pointerDownContent.eventID = EventTriggerType.PointerDown;
                pointerDownContent.callback.AddListener((e) => OnMouseDown());

                var pointerUpContent = triggerContent.triggers[1];// new EventTrigger.Entry();
                pointerUpContent.eventID = EventTriggerType.PointerUp;
                pointerUpContent.callback.AddListener((e) => OnMouseUp());

                var pointerEnterContent = triggerContent.triggers[2];
                pointerEnterContent.eventID = EventTriggerType.PointerEnter;
                pointerEnterContent.callback.AddListener((e) => OnMouseOver());

                var pointerExitContent = triggerContent.triggers[3];
                pointerExitContent.eventID = EventTriggerType.PointerExit;
                pointerExitContent.callback.AddListener((e) => OnMouseExit());
            }

            for (int i = 0; i < sizers.Length; i++)
            {
                /*OldTriggers = sizers[i].gameObject.GetComponents<EventTrigger>();
                for (int j = 1; j < OldTriggers.Length; j++)
                {
                    Destroy(OldTriggers[j]);
                }*/
                EventTrigger triggerSizer = sizers[i].gameObject.GetComponent<EventTrigger>();
                var pointerDownSizer = triggerSizer.triggers[0];// new EventTrigger.Entry();
                pointerDownSizer.eventID = EventTriggerType.PointerDown;
                int _i = i;
                pointerDownSizer.callback.AddListener((e) => OnMouseDownSize(_i));
                //triggerSizer.triggers.Add(pointerDownSizer);

                var pointerUpSizer = triggerSizer.triggers[1];// new EventTrigger.Entry();
                pointerUpSizer.eventID = EventTriggerType.PointerUp;
                pointerUpSizer.callback.AddListener((e) => OnMouseUpSize());
                //triggerSizer.triggers.Add(pointerUpSizer);
                //triggers.Add(triggerSizer);

                var pointerEnterSizer = triggerSizer.triggers[2];// new EventTrigger.Entry();
                pointerEnterSizer.eventID = EventTriggerType.PointerEnter;
                _i = i;
                pointerEnterSizer.callback.AddListener((e) => OnMouseOverSize(_i));
                //triggerSizer.triggers.Add(pointerDownSizer);

                var pointerExitSizer = triggerSizer.triggers[3];// new EventTrigger.Entry();
                pointerExitSizer.eventID = EventTriggerType.PointerExit;
                pointerExitSizer.callback.AddListener((e) => OnMouseExitSize());
                //triggerSizer.triggers.Add(pointerUpSizer);
                //triggers.Add(triggerSizer);
            }
            currSize = currRect.sizeDelta;
        }

        
        public virtual void Update()
        {
            ChangeSizeBorder();
            Move();
            ChangeSizeWindow();
            if (isRotate)
            {
                Rect rect = UIScreenTool.UIToScreenRect(canvas, currRect);
                transform.rotation = new Quaternion((rect.y + rect.height / 2 - 0.5f) / 4, (rect.x + rect.width / 2 - 0.5f) / 2, 0, 1);
            }

        }

        public void ChangeSizeBorder()
        {
            if (SizeBorders != _sizeBorders)
            {
                _sizeBorders = SizeBorders;
                for (int i = 0; i < 4; i++)
                {
                    sizers[i].GetComponent<RectTransform>().sizeDelta = new Vector2(_sizeBorders, _sizeBorders);
                }
                sizers[4].GetComponent<RectTransform>().sizeDelta = new Vector2(_sizeBorders, sizers[4].GetComponent<RectTransform>().sizeDelta.y);
                sizers[5].GetComponent<RectTransform>().sizeDelta = new Vector2(_sizeBorders, sizers[5].GetComponent<RectTransform>().sizeDelta.y);
                sizers[6].GetComponent<RectTransform>().sizeDelta = new Vector2(sizers[6].GetComponent<RectTransform>().sizeDelta.x, _sizeBorders);
                sizers[7].GetComponent<RectTransform>().sizeDelta = new Vector2(sizers[7].GetComponent<RectTransform>().sizeDelta.x, _sizeBorders);
                content.offsetMin = new Vector2(_sizeBorders, _sizeBorders);
                content.offsetMax = new Vector2(-_sizeBorders, -header.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        public virtual bool Move()
        {
            if (isDrag && dragIndex == -1)
            {
                transform.SetAsLastSibling();
                Vector3 delta = UIScreenTool.ScreenToUIPos(canvas, new Vector2(Input.mousePosition.x, Input.mousePosition.y)) -
                    UIScreenTool.ScreenToUIPos(canvas, lastMovePos);

                if (true/*currRect.anchoredPosition.x + delta.x >= 0 && currRect.anchoredPosition.y + delta.y >= 0 &&
                currRect.anchoredPosition.x + currRect.sizeDelta.x + delta.x < 1080 &&
                currRect.anchoredPosition.y + currRect.sizeDelta.y + delta.y < 1080*/)
                {
                    currRect.position += delta;
                }
                lastMovePos = Input.mousePosition;
                return true;
            }
            return false;
        }

        public virtual bool ChangeSizeWindow()
        {
            if (dragIndex >= 0)
            {
                Vector2 lastPos = currRect.position;
                transform.SetAsLastSibling();
                Vector2 delta = UIScreenTool.ScreenToUIPos(canvas, new Vector2(Input.mousePosition.x, Input.mousePosition.y)) -
                    UIScreenTool.ScreenToUIPos(canvas, lastSizePos);
                if (dragIndex == 0 || dragIndex == 2 || dragIndex == 4)
                {
                    if (currRect.rect.width - 20 > delta.x * 1 / currRect.transform.lossyScale.x)
                    //currRect.anchoredPosition.x + delta.x >= 0)
                    {
                        currRect.position += delta.x * Vector3.right * (1 - currRect.pivot.x);
                        currSize -= delta.x * Vector2.right * 1 / currRect.transform.lossyScale.x;
                    }
                }
                else if (dragIndex == 1 || dragIndex == 3 || dragIndex == 5)
                {
                    //currRect.anchoredPosition += delta.x * Vector2.right;
                    if (currRect.rect.width - 20 > -delta.x * 1 / currRect.transform.lossyScale.x)
                    //currRect.anchoredPosition.x + currRect.sizeDelta.x + delta.x < 1080)
                    {
                        currRect.position += delta.x * Vector3.right * (1 - currRect.pivot.x);
                        currSize += delta.x * Vector2.right * 1 / currRect.transform.lossyScale.x;
                    }
                }

                if (dragIndex == 0 || dragIndex == 1 || dragIndex == 6)
                {
                    if (currRect.rect.height - 50 > -delta.y * 1 / currRect.transform.lossyScale.y)
                    //currRect.anchoredPosition.y + currRect.sizeDelta.y + delta.y < 1080)
                    {
                        currRect.position += delta.y * Vector3.up * (1 - currRect.pivot.y);
                        currSize += delta.y * Vector2.up * 1 / currRect.transform.lossyScale.y;
                    }
                }
                else if(dragIndex == 2 || dragIndex == 3 || dragIndex == 7)
                {
                    if (currRect.rect.height - 50 > delta.y * 1 / currRect.transform.lossyScale.y)
                    //currRect.anchoredPosition.y + delta.y >= 0)
                    {
                        currRect.position += delta.y * Vector3.up * (1 - currRect.pivot.y);
                        currSize -= delta.y * Vector2.up * 1 / currRect.transform.lossyScale.y;
                    }
                }
                if (currSize.x < minSize.x)
                    currSize.x = minSize.x;
                if (currSize.y < minSize.y)
                    currSize.y = minSize.y;
                currRect.sizeDelta = currSize;
                lastSizePos = Input.mousePosition;
                return true;
            }
            return false;
        }

        bool isDrag = false;
        Vector2 lastMovePos;
        public void OnMouseDown()//OnMouseDrag()
        {
            isDrag = true;
            lastMovePos = Input.mousePosition;
        }

        public void OnMouseUp()//OnMouseDrag()
        {
            isDrag = false;
        }

        public void OnMouseOver()//OnMouseDrag()
        {
            CursorLogic.cursorType = CursorLogic.CursorType.MOVE;
        }

        public void OnMouseExit()//OnMouseDrag()
        {
            CursorLogic.cursorType = CursorLogic.CursorType.SIMPLE;
        }

        int dragIndex = -1;
        int overIndex = -1;
        Vector2 lastSizePos;

        public void OnMouseDownSize(int index)//OnMouseDrag()
        {
            dragIndex = index;
            lastSizePos = Input.mousePosition;
        }

        public void OnMouseUpSize()//OnMouseDrag()
        {
            dragIndex = -1;
			if (overIndex < 0) 
			{
				CursorLogic.cursorType = CursorLogic.CursorType.SIMPLE;
				CursorLogic.index = 0;
			}
        }

        public void OnMouseOverSize(int index)
        {
			if (dragIndex >= 0)
				return;
            overIndex = index;
            CursorLogic.cursorType = CursorLogic.CursorType.RESIZE;
			switch(index)
			{
				case 0: case 3:
					CursorLogic.index = 3;
					break;
				case 1: case 2:
					CursorLogic.index = 1;
					break;
				case 4: case 5:
					CursorLogic.index = 2;
					break;
				default:
					CursorLogic.index = 0;
					break;
			}
        }

        public void OnMouseExitSize()
        {
            overIndex = -1;
            if(dragIndex < 0)
			{
                CursorLogic.cursorType = CursorLogic.CursorType.SIMPLE;
				CursorLogic.index = 0;
			}
        }

        void OnDestoy()
        {
            foreach (var trigger in triggers)
            {
                Destroy(trigger);
            }
        }
    }
}
