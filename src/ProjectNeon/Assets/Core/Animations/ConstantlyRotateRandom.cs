using UnityEngine;

public class ConstantlyRotateRandom : MonoBehaviour
{
    [SerializeField] private Vector3 factor = new Vector3(1, 1, 1);

    private Vector3 rotationSpeed;

    private void Awake()
    {
        rotationSpeed = new Vector3(Rng.Float() * factor.x, Rng.Float() * factor.y, Rng.Float() * factor.z);
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotationSpeed.x * Time.deltaTime, rotationSpeed.y * Time.deltaTime, rotationSpeed.z * Time.deltaTime));
    }
}
