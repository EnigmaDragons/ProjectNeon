using UnityEngine;
using UnityEngine.UI;

public class WithFlashingTint : MonoBehaviour
{
    [SerializeField] private float amount = 0.3f;
    [SerializeField] private float duration = 0.75f;

    private readonly float _originalAmount = 1.0f;
    private readonly Color _original = Color.white;
    private Color _darker;
    private Image _sprite;

    private void Start()
    {
        _sprite = GetComponent<Image>();
        _darker = new Color(_originalAmount - amount, _originalAmount - amount, _originalAmount - amount, _sprite.color.a);
    }

    void Update()
    {
        _sprite.color = Color.Lerp(_original, _darker, Mathf.PingPong(Time.time, duration));
    }
}
