using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollbarBtn : MonoBehaviour {

    public Button leftBtn;
    public Button rightBtn;
    public float step = 0.1f;

    private Scrollbar _scroll;

    void DeltaScrollbar(float delta)
    {
        _scroll.value = _scroll.value + delta;
    }

    // Use this for initialization
    void Start () {
        _scroll = GetComponent<Scrollbar>();
        leftBtn.onClick.AddListener(() => DeltaScrollbar(-step));
        rightBtn.onClick.AddListener(() => DeltaScrollbar(step));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
