using UnityEngine;

public class FlyingUpwards : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxLifetime;

    private float _remainingLife;

    private void OnEnable()
    {
        _remainingLife = maxLifetime;
    }
    
    private void Update()
    {
        if (_remainingLife <= 0)
            return;
        
        _remainingLife += Time.deltaTime;
        transform.localPosition += new Vector3(0, Time.deltaTime * speed, 0);
        if (_remainingLife <= 0)
            Destroy(this);
    }
}
