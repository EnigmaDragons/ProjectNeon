using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public sealed class Stopwatch : MonoBehaviour
{
    private TextMeshProUGUI display;

    private float elapsed;
    
    void Awake() => display = GetComponent<TextMeshProUGUI>();

    private void FixedUpdate()
    {
        elapsed += Time.deltaTime;
        display.text = elapsed.ToString("n2");
    }
}
