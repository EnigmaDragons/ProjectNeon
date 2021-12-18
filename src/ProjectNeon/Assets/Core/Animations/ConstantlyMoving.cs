using UnityEngine;

public class ConstantlyMoving : MonoBehaviour
{
    [SerializeField] private Vector3 velocity;

    public void SetVelocity(Vector3 newVelocity) => velocity = newVelocity;

    private void FixedUpdate() => transform.position += velocity * Time.deltaTime;
}