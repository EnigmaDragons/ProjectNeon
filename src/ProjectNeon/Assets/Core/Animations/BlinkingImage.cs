using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlinkingImage : MonoBehaviour
{
    [SerializeField] private float minAlpha = 32;
    [SerializeField] private float duration = 0.75f;

    private Color _original;
    private Color _final;
    private Image _img;

    private void Start()
    {
        _img = GetComponent<Image>();
        _original = _img.color;
        _final = _original.WithAlpha(minAlpha);
    }

    void Update()
    {
        _img.color = Color.Lerp(_original, _final, Mathf.PingPong(Time.time, duration));
    }
}
