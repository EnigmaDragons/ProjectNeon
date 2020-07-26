using UnityEngine;
 
public sealed class Floating : MonoBehaviour 
{
    [SerializeField] private float amplitude = 0.5f;
    [SerializeField] private float frequency = 1f;
    
    private Vector3 _posOffset = new Vector3();

    private void OnEnable()
    {
        _posOffset = transform.position;
    }

    private void FixedUpdate ()
    {
        transform.position = _posOffset + new Vector3(0, Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude, 0);
    }
}
