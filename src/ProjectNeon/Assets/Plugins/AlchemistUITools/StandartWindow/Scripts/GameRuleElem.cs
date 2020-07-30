using UnityEngine;
using System.Collections;

namespace UITool
{
    public class GameRuleElem : MonoBehaviour
    {

        public string id;
        [SerializeField] protected Canvas canvas;

        void Start()
        {
            if (PlayerPrefs.HasKey("RuleElem_" + id + "_x"))
            {
                Rect screenRect = new Rect(PlayerPrefs.GetFloat("RuleElem_" + id + "_x"), PlayerPrefs.GetFloat("RuleElem_" + id + "_y"), PlayerPrefs.GetFloat("RuleElem_" + id + "_width"), PlayerPrefs.GetFloat("RuleElem_" + id + "_height"));
                screenRect.height *= 1f * Screen.width / Screen.height;
                UIScreenTool.ScreenToUIRect(canvas, screenRect, GetComponent<RectTransform>(), true);
            }
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}
