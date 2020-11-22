using UnityEngine;
 
public sealed class Floating : MonoBehaviour 
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;

    private void FixedUpdate ()
    {
        transform.Translate(new Vector3(0, Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude, 0));
    }
}
