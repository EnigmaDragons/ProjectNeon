using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SmartToggle : MonoBehaviour
{

    public Image[] onImages;
    public Image[] offImages;
    public RectTransform movedElem;
    public Transform onPos;
    public Transform offPos;
    public float speed = 1;

    private Toggle _toggle;
    private float _koef = 0;
    private bool _isSwitched = false;

    void OnChangeValue(bool value)
    {
        if (!_isSwitched)
        {
            _isSwitched = true;
            for (int i = 0; i < onImages.Length; i++)
                onImages[i].enabled = true;
            for (int i = 0; i < offImages.Length; i++)
                offImages[i].enabled = true;
        }
    }

    // Use this for initialization
    void Start ()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener((value) => OnChangeValue(value));
        _koef = _toggle.isOn ? 1 : 0;
        OnChangeValue(_toggle.isOn);
        UpdateAnim(100);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnim(Time.deltaTime * speed);
    }

    void UpdateAnim(float delta)
    {
        if (_isSwitched)
        {
            _koef += (_toggle.isOn ? 1 : -1) * delta;
            _koef = Mathf.Max(0, Mathf.Min(1, _koef));

            for (int i = 0; i < onImages.Length; i++)
            {
                Color color = onImages[i].color;
                color.a = _koef;
                onImages[i].color = color;
            }
            for (int i = 0; i < offImages.Length; i++)
            {
                Color color = offImages[i].color;
                color.a = 1 - _koef;
                offImages[i].color = color;
            }
            if (movedElem != null)
                movedElem.position = Vector3.Lerp(offPos.position, onPos.position, _koef);

            if ((_koef >= 1 && _toggle.isOn) || (_koef <= 0 && !_toggle.isOn))
            {
                _isSwitched = false;
                for (int i = 0; i < onImages.Length; i++)
                    onImages[i].enabled = _toggle.isOn;
                for (int i = 0; i < offImages.Length; i++)
                    offImages[i].enabled = !_toggle.isOn;
            }
        }
	}
}
