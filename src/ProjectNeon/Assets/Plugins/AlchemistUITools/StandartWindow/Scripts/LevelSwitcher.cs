using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelSwitcher : MonoBehaviour {

    [System.Serializable]
    public class LevelButton
    {
        public Button btn;
        public int indexLevel;
    }

    public LevelButton[] buttons;

	// Use this for initialization
	void Start () {
	    for(int i = 0; i < buttons.Length; i++)
        {
            int _i = i;
            buttons[i].btn.onClick.AddListener(() => AnimSwitchScene.LoadLevel(buttons[_i].indexLevel));
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
