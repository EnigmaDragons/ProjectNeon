using UnityEngine;

public class OnStartAdjustTransform : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Vector3 scale;
    
    private void Start()
    {
        transform.position += position;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + rotation);
        transform.localScale += scale;
    }
}
