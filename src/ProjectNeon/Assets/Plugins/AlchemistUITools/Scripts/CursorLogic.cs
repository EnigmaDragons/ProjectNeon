using UnityEngine;
using System.Collections;

public class CursorLogic : MonoBehaviour {

    public enum CursorType { SIMPLE, RESIZE, MOVE }
    public static CursorType cursorType = CursorType.SIMPLE;
	public static int index = 0;

    public GameObject simpleCursor;
    public GameObject resizeCursor;
    public GameObject miniMoveCursor;

    private RectTransform _rect;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
        _rect = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            simpleCursor.SetActive(cursorType != CursorType.RESIZE);
            resizeCursor.SetActive(cursorType == CursorType.RESIZE);
            miniMoveCursor.SetActive(cursorType == CursorType.MOVE);
        }
        _rect.anchoredPosition = new Vector2(Input.mousePosition.x * 1080 / Screen.height, Input.mousePosition.y * 1080 / Screen.height);
        _rect.GetChild(1).localRotation = Quaternion.identity;
        _rect.GetChild(1).Rotate(new Vector3(0, 0, -45 * index));
	}
}
