using UnityEngine;

public class ConstantlyRotate : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(1, 1, 1);

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotationSpeed.x * Time.deltaTime, rotationSpeed.y * Time.deltaTime, rotationSpeed.z * Time.deltaTime));
    }
}
