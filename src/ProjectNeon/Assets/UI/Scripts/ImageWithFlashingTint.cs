using UnityEngine;
using UnityEngine.UI;

public class ImageWithFlashingTint : MonoBehaviour
{
    [SerializeField] private float amount = 0.3f;
    [SerializeField] private float duration = 0.75f;

    private readonly float _originalAmount = 1.0f;
    private readonly Color _original = Color.white;
    private Color _darker;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        _darker = new Color(_originalAmount - amount, _originalAmount - amount, _originalAmount - amount, _image.color.a);
    }

    void Update()
    {
        _image.color = Color.Lerp(_original, _darker, Mathf.PingPong(Time.time, duration));
    }
}
