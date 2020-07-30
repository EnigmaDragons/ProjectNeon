using UnityEngine;
using System.Collections;

namespace UITool
{
    public class RuleElemLogic : WindowLogic
    {
        public string id;

        //private Vector2 defaultPosition;
        //private Vector2 defaultSize;
       
        void Start()
        {
            base.Start();
            /*defaultPosition = currRect.position;
            defaultSize = currRect.sizeDelta;
            if (PlayerPrefs.HasKey("RuleElem_" + id + "_x"))
            {
                Rect screenRect = new Rect(PlayerPrefs.GetFloat("RuleElem_" + id + "_x"), PlayerPrefs.GetFloat("RuleElem_" + id + "_y"), PlayerPrefs.GetFloat("RuleElem_" + id + "_width"), PlayerPrefs.GetFloat("RuleElem_" + id + "_height"));
                screenRect.height *= 1f * Screen.width / Screen.height;
                UIScreenTool.ScreenToUIRect(canvas, screenRect, currRect, true);
            }*/
			if(Application.isPlaying)
				SaveCoord ();
        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        public override bool Move()
        {
            bool ret = base.Move();
            if(ret)
            {
                SaveCoord();
            }
            return ret;
        }

        public override bool ChangeSizeWindow()
        {
            bool ret = base.ChangeSizeWindow();
            if (ret)
            {
                SaveCoord();
            }
            return ret;
        }

        private void SaveCoord()
        {
            Rect rect = UIScreenTool.UIToScreenRect(canvas, currRect);
            rect.height = rect.height * Screen.height / Screen.width;
            PlayerPrefs.SetFloat("RuleElem_" + id + "_x", rect.x);
            PlayerPrefs.SetFloat("RuleElem_" + id + "_y", rect.y);
            PlayerPrefs.SetFloat("RuleElem_" + id + "_width", rect.width);
            PlayerPrefs.SetFloat("RuleElem_" + id + "_height", rect.height);
        }
    }
}
